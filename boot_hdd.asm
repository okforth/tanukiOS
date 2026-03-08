BITS 16
CPU 8086
org 0x7c00

jmp start

; FAT headers stops at 3e
times 0x3e - ( $ - $$ ) db 0

bootdrive: db 0

; FLOPPY BOOT
; =============================================================
;  360KB 5.25" double density: 40 cylinders, 2 head,  9 sectors
;  1.2MB 5.25" high density:   80 cylinders, 2 head, 15 sectors
;  720KB 3.5"  double density: 80 cylinders, 2 head,  9 sectors
; 1.44MB 3.5"  high density:   80 cylinders, 2 head, 18 sectors
; =============================================================

; USB BOOT: 1023 cylinders, 16 head, 63 sectors

%define MAX_CYLINDER 1023
%define MAX_HEAD 16
%define MAX_SECTOR 63

start:
	cld			; clear direction flag
	mov [bootdrive], dl	; store bootdrive for later

	; set up hardware stack
	xor ax, ax
	mov ds, ax
	mov es, ax
	mov ss, ax
	mov sp, ax

	mov ax, 0x0003		; set video mode 80x25 color text
	int 0x10

	mov si, hello
	call print

	; load kernel
	mov	ah, 0x02
	mov	al, 17		; sector count
	mov	ch, 0		; cylinder
	mov	cl, 2		; sector
	mov	dh, 0		; head
				; dl = boot drive
	mov	bx, 0x0500	; destination address
	int	0x13

	; load source
	mov	ah, 0x02
	mov	al, 64		; sector count
	mov	ch, 0		; cylinder
	mov	cl, 19		; sector
	mov	dh, 0		; head
				; dl = boot drive
	mov	bx, 0x7e00	; destination address
	int	0x13

;	mov	ah, 0x02
;	mov	al, 20		; sector count
;	mov	ch, 1		; cylinder
;	mov	cl, 1		; sector
;	mov	dh, 1		; head
;				; dl = boot drive
;	add	bx, 512*44	; destination address
;	int	0x13

	jmp 0:0x0500		; go to kernel

disk_error:
	mov si, error
	call print
	jmp $

	; print null-terminated string
	; si = string address
print:	lodsb
	or al, al
	jz .done
	mov ah, 0x0e
	int 0x10
	jmp print
.done:	ret

hello:	db 'Loading...', 13, 10, 0
error:	db 'Error loading disk.', 13, 10, 0

	; magic numbers
	times 510 - ( $ - $$ ) db 0
	db 0x55, 0xaa
