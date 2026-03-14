default: floppy

boot_fdd.bin: boot_fdd.asm
	nasm $< -o $@

boot_hdd.bin: boot_hdd.asm
	nasm $< -o $@

# 17 sectors
kernel.bin: kernel.asm
	nasm $< -o $@
	truncate -s $$((512 * 17)) $@

# 64 sectors / 32 blocks
blocks.bin: blocks.fs blocks.py
	python3 blocks.py $< $@
	truncate -s $$((512 * 64)) $@

# 1.44MB (3.5") floppy = 2880 sectors
# 360KB (5.25") floppy = 720 sectors
fdd.img: boot_fdd.bin kernel.bin blocks.bin
	dd if=/dev/zero of=$@ bs=512 count=720
	dd if=boot_fdd.bin of=$@ bs=512 count=1 conv=notrunc
	dd if=kernel.bin of=$@ bs=512 seek=1 conv=notrunc
	dd if=blocks.bin of=$@ bs=512 seek=18 conv=notrunc

hdd.img: boot_hdd.bin kernel.bin blocks.bin
	dd if=/dev/zero of=$@ bs=512 count=720
	dd if=boot_hdd.bin of=$@ bs=512 count=1 conv=notrunc status=progress
	dd if=kernel.bin of=$@ bs=512 seek=1 conv=notrunc status=progress
	dd if=blocks.bin of=$@ bs=512 seek=18 conv=notrunc status=progress

fdd: fdd.img
	qemu-system-i386 \
	-drive file=$<,if=floppy,format=raw # -boot a

hdd: hdd.img
	qemu-system-i386 \
	-drive file=$<,format=raw

load: hdd.img
	diskutil unmountDisk /dev/disk4
	sudo dd if=$< of=/dev/rdisk4 bs=512 status=progress
	sync
	diskutil eject /dev/disk4

clean:
	rm *.bin
	rm *.img

floppy: fdd

usb: hdd.img
