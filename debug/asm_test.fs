\ Test suite for the built-in assembler subsystem in "blocks.fs"
\ My original, more complete, test suite was lost due a human
\ error. So, for the sake of my own sanity, I have used Lubomir
\ Rintel's 86ASM.FTH Programmer’s Manual as my reference.

\ https://github.com/lkundrak/86asm.fth

\ You can add your own tests in this matter:
\ -> [assembly code] >> [expected bytes]
\ The test will return an error if the assembled bytes (compiled
\ to HERE) do not match with the explicitly defined expected
\ bytes. The length of the expected bytes must match with the
\ length of the assembled bytes.

\ ASSEMBLER TEST
: -> ( -- addr ) here @ ;
: range ( addr -- addr count ) dup here @ -^ ;
: ?err ( f -- ) if abort" mismatched bytes" then ;
: cmp? ( byte -- f ) word parse drop = not ?err ;
: see ( byte -- ) .x space ;
: wait ( -- ) key? if drop exit then recurse ;
: ? depth 0= not if abort" bytes too short" then ;
: >> wait range >r begin c@+ dup see cmp? next drop ? cr ;
\ MOV
-> $1234 [DI] MOV            >> $c7 $05 $34 $12
-> $34 [SI] SHORT MOV        >> $c6 $04 $34
-> $1234 5 +[BX+SI] MOV      >> $c7 $40 $05 $34 $12
-> CX [DI] MOV               >> $89 $0d
-> [DI] CX MOV               >> $8b $0d
-> BX CX MOV                 >> $89 $d9
-> CX BX MOV                 >> $89 $cb
-> CX $12 PTR MOV            >> $89 $0e $12 $00
-> $12 PTR CX MOV            >> $8b $0e $12 $00
-> $12 5 +[BX+SI] SHORT MOV  >> $c6 $40 $05 $12
-> $34 [SI] LONG MOV         >> $c7 $04 $34 $00
-> $1234 [DI] LONG MOV       >> $c7 $05 $34 $12
-> $1234 5 +[BX+SI] LONG MOV >> $c7 $40 $05 $34 $12
-> $34 [SI] MOV              >> $c7 $04 $34 $00
-> CL [DI] MOV               >> $88 $0d
-> [DI] CL MOV               >> $8a $0d
-> BL CL MOV                 >> $88 $d9
-> CL BL MOV                 >> $88 $cb
-> CL $12 PTR MOV            >> $88 $0e $12 $00
-> $12 PTR CL MOV            >> $8a $0e $12 $00
-> $1234 PTR AX MOV          >> $a1 $34 $12
-> AX $1234 PTR MOV          >> $a3 $34 $12
-> $1234 PTR AL MOV          >> $a0 $34 $12
-> AL $1234 PTR MOV          >> $a2 $34 $12
-> $1234 BX MOV              >> $bb $34 $12
-> $12 BL MOV                >> $b3 $12
cr \ PUSH, POP
-> BX PUSH          >> $53
-> $1234 +[DI] PUSH >> $ff $b5 $34 $12
-> [BX+SI] PUSH     >> $ff $30
-> $1234 PTR PUSH   >> $ff $36 $34 $12
-> $12 +[DI] PUSH   >> $ff $75 $12
-> [DI] PUSH        >> $ff $35
-> [BP] PUSH        >> $ff $76 $00
-> BX POP           >> $5b
-> $1234 +[DI] POP  >> $8f $85 $34 $12
-> [BX+SI] POP      >> $8f $00
-> $1234 PTR POP    >> $8f $06 $34 $12
-> $12 +[DI] POP    >> $8f $45 $12
-> [DI] POP         >> $8f $05
-> [BP] POP         >> $8f $46 $00    
cr \ XCHG, NOP
-> DX AX XCHG          >> $92
-> AX DX XCHG          >> $92
-> BX BX XCHG          >> $87 $db
-> $1234 +[DI] BX XCHG >> $87 $9d $34 $12
-> [BX+SI] BX XCHG     >> $87 $18
-> $1234 PTR BX XCHG   >> $87 $1e $34 $12
-> [DI] BX XCHG        >> $87 $1d
-> $12 +[DI] BX XCHG   >> $87 $5d $12
-> [BP] BX XCHG        >> $87 $5e $00
-> BX $1234 +[DI] XCHG >> $87 $9d $34 $12
-> BX [BX+SI] XCHG     >> $87 $18
-> BX [DI] XCHG        >> $87 $1d
-> BX $12 +[DI] XCHG   >> $87 $5d $12
-> BX [BP] XCHG        >> $87 $5e $00
-> BX $1234 PTR XCHG   >> $87 $1e $34 $12
-> AX AX XCHG          >> $90
-> NOP                 >> $90
cr \ IN, OUT
-> $30 AX IN  >> $e5 $30
-> DX AX IN   >> $ed
-> $30 AL IN  >> $e4 $30
-> DX AL IN   >> $ec
-> AX $30 OUT >> $e7 $30
-> AX DX OUT  >> $ef
-> AL $30 OUT >> $e6 $30
-> AL DX OUT  >> $ee
cr \ ADD, ADC, SUB, SSB
-> $12 AX ADD   >> $05 $12 $00 
-> $1234 AX ADD >> $05 $34 $12 
-> $12 AL ADD   >> $04 $12 
-> $12 BX ADD   >> $81 $c3 $12 $00
-> $1234 BX ADD >> $81 $c3 $34 $12
-> $12 BL ADD   >> $80 $c3 $12 
-> CX BX ADD    >> $01 $cb
-> $12 AX ADC   >> $15 $12 $00 
-> $1234 AX ADC >> $15 $34 $12 
-> $12 AL ADC   >> $14 $12 
-> $12 BX ADC   >> $81 $d3 $12 $00
-> $1234 BX ADC >> $81 $d3 $34 $12
-> $12 BL ADC   >> $80 $d3 $12 
-> CX BX ADC    >> $11 $cb 
-> $12 AX SUB   >> $2d $12 $00 
-> $1234 AX SUB >> $2d $34 $12 
-> $12 AL SUB   >> $2c $12 
-> $12 BX SUB   >> $81 $eb $12 $00
-> $1234 BX SUB >> $81 $eb $34 $12
-> $12 BL SUB   >> $80 $eb $12 
-> CX BX SUB    >> $29 $cb 
-> $12 AX SBB   >> $1d $12 $00 
-> $1234 AX SBB >> $1d $34 $12 
-> $12 AL SBB   >> $1c $12 
-> $12 BX SBB   >> $81 $db $12 $00
-> $1234 BX SBB >> $81 $db $34 $12
-> $12 BL SBB   >> $80 $db $12 
-> CX BX SBB    >> $19 $cb        
cr \ MUL, IMUL, DIV, IDIV
-> BX MUL                 >> $f7 $e3 
-> CL MUL                 >> $f6 $e1 
-> [DI] MUL               >> $f7 $25 
-> $1234 +[DI] SHORT MUL  >> $f6 $a5 $34 $12
-> BX IMUL                >> $f7 $eb 
-> CL IMUL                >> $f6 $e9 
-> [DI] IMUL              >> $f7 $2d 
-> $1234 +[DI] SHORT IMUL >> $f6 $ad $34 $12
-> BX DIV                 >> $f7 $f3 
-> CL DIV                 >> $f6 $f1 
-> [DI] DIV               >> $f7 $35 
-> $1234 +[DI] SHORT DIV  >> $f6 $b5 $34 $12
-> BX IDIV                >> $f7 $fb 
-> CL IDIV                >> $f6 $f9 
-> [DI] IDIV              >> $f7 $3d 
-> $1234 +[DI] SHORT IDIV >> $f6 $bd $34 $12
cr \ INC, DEC, NEG, NOT
-> BX INC                >> $43 
-> CL INC                >> $fe $c1 
-> [DI] INC              >> $ff $05 
-> $1234 +[DI] SHORT INC >> $fe $85 $34 $12
-> [BX+SI] SHORT INC     >> $fe $00 
-> BX DEC                >> $4b 
-> BL DEC                >> $fe $cb 
-> [DI] DEC              >> $ff $0d 
-> [DI] SHORT DEC        >> $fe $0d 
-> BX NEG                >> $f7 $db 
-> BL NEG                >> $f6 $db 
-> [DI] NEG              >> $f7 $1d 
-> [DI] SHORT NEG        >> $f6 $1d 
-> [DI] NOT              >> $f7 $15 
-> BL NOT                >> $f6 $d3 
-> BX NOT                >> $f7 $d3 
-> CL NOT                >> $f6 $d1 
-> $1234 +[DI] SHORT NOT >> $f6 $95 $34 $12
-> [BX+SI] SHORT NOT     >> $f6 $10
cr \ CMP
-> $12 AX CMP   >> $3d $12 $00 
-> $1234 AX CMP >> $3d $34 $12 
-> $12 AL CMP   >> $3c $12 
-> $12 BX CMP   >> $81 $fb $12 $00
-> $1234 BX CMP >> $81 $fb $34 $12
-> $12 BL CMP   >> $80 $fb $12 
-> CX BX CMP    >> $39 $cb        
cr \ CBW, CWD
-> CBW >> $98
-> CWD >> $99
cr \ SHL, SHR, SAR, ROL, ROR, RCL, RCR
-> $1 BX SHL                >> $d1 $e3
-> $1 BL SHL                >> $d0 $e3
-> CL $5 +[BX+SI] SHL       >> $d3 $60 $05
-> $1 $5 +[BX+SI] SHORT SHL >> $d0 $60 $05
-> $1 BX SHR                >> $d1 $eb
-> $1 BL SHR                >> $d0 $eb
-> CL $5 +[BX+SI] SHR       >> $d3 $68 $05
-> $1 $5 +[BX+SI] SHORT SHR >> $d0 $68 $05
-> $1 BX SAR                >> $d1 $fb
-> $1 BL SAR                >> $d0 $fb
-> CL $5 +[BX+SI] SAR       >> $d3 $78 $05
-> $1 $5 +[BX+SI] SHORT SAR >> $d0 $78 $05
-> $1 BX ROL                >> $d1 $c3
-> $1 BL ROL                >> $d0 $c3
-> CL $5 +[BX+SI] ROL       >> $d3 $40 $05
-> $1 $5 +[BX+SI] SHORT ROL >> $d0 $40 $05
-> $1 BX ROR                >> $d1 $cb
-> $1 BL ROR                >> $d0 $cb
-> CL $5 +[BX+SI] ROR       >> $d3 $48 $05
-> $1 $5 +[BX+SI] SHORT ROR >> $d0 $48 $05
-> $1 BX RCL                >> $d1 $d3
-> $1 BL RCL                >> $d0 $d3
-> CL $5 +[BX+SI] RCL       >> $d3 $50 $05
-> $1 $5 +[BX+SI] SHORT RCL >> $d0 $50 $05
-> $1 BX RCR                >> $d1 $db
-> $1 BL RCR                >> $d0 $db
-> CL $5 +[BX+SI] RCR       >> $d3 $58 $05
-> $1 $5 +[BX+SI] SHORT RCR >> $d0 $58 $05
\ AND, TEST, OR, XOR
-> $12 AX AND    >> $25 $12 $00
-> $1234 AX AND  >> $25 $34 $12
-> $12 AL AND    >> $24 $12
-> $12 BX AND    >> $81 $e3 $12 $00
-> $1234 BX AND  >> $81 $e3 $34 $12
-> $12 BL AND    >> $80 $e3 $12
-> CX BX AND     >> $21 $cb
-> $12 AX TEST   >> $a9 $12 $00
-> $1234 AX TEST >> $a9 $34 $12
-> $12 AL TEST   >> $a8 $12
-> $12 BX TEST   >> $f7 $c3 $12 $00
-> $1234 BX TEST >> $f7 $c3 $34 $12
-> $12 BL TEST   >> $f6 $c3 $12
-> CX BX TEST    >> $85 $cb
-> $12 AX OR     >> $0d $12 $00
-> $1234 AX OR   >> $0d $34 $12
-> $12 AL OR     >> $0c $12
-> $12 BX OR     >> $81 $cb $12 $00
-> $1234 BX OR   >> $81 $cb $34 $12
-> $12 BL OR     >> $80 $cb $12
-> CX BX OR      >> $09 $cb
-> $12 AX XOR    >> $35 $12 $00
-> $1234 AX XOR  >> $35 $34 $12
-> $12 AL XOR    >> $34 $12
-> $12 BX XOR    >> $81 $f3 $12 $00
-> $1234 BX XOR  >> $81 $f3 $34 $12
-> $12 BL XOR    >> $80 $f3 $12
-> CX BX XOR     >> $31 $cb
cr \ CALL, JMP
-> 5 +[BX+SI] CALL >> $ff $50 $05
-> 5 +[BX+SI] CALL >> $ff $50 $05
-> [BX] CALL       >> $ff $17
-> $12 PTR CALL    >> $ff $16 $12 $00
-> $1234 PTR CALL  >> $ff $16 $34 $12
-> $1234 CALL      >> $e8 $34 $12
-> 5 +[BX+SI] JMP  >> $ff $60 $05
-> [BX] JMP        >> $ff $27
-> $12 PTR JMP     >> $ff $26 $12 $00
-> $1234 PTR JMP   >> $ff $26 $34 $12
-> $12 JMP         >> $eb $12
-> $1234 JMP       >> $e9 $34 $12
