# gnazo_save_manager
### Mar 6th 2018
Save file manager for Gensokyo no Nazo

Download the latest release from [here](https://github.com/shadax1/gnazo_save_manager/releases) into your game folder. You can either start the game followed by the practice tool or start the practice tool which will automatically start the game.

**Note:** your game must be version 1.00 for the tool to display 'Speed', 'HP', 'Framecount/Previous' values. Otherwise, the save file management feature can also be used with 1.01.

![demo pic](https://raw.githubusercontent.com/shadax1/gnazo_save_manager/master/demo%20pic.png)

## Memory addresses used
All addresses are static which makes coding this very simple:
```csharp
gnazo.exe+84FFDC -> lives
gnazo.exe+928596 -> speed
gnazo.exe+B8D0E4 -> timer
gnazo.exe+4408E4 -> loading flag

gnazo.exe+8533D8+(44*idChar)+offset

44      => length of char info
idChar  => starts from 0 (reimu) up to 24 (suwako) -> gnazo.exe+B8D0EC
offset  => one of the bytes listed below

gnazo.exe+8533D8+(44*16)+10 => reisen's maxRP for instance

0  - 0x00 - curHP               -   gnazo.exe+8533D8+(44*0)+0
4  - 0x04 - maxHP               -   gnazo.exe+8533D8+(44*0)+4
14 - 0x0E - curRP1*2            -   gnazo.exe+8533D8+(44*0)+14
16 - 0x10 - maxRP               -   gnazo.exe+8533D8+(44*0)+16
20 - 0x14 - red orbs            -   gnazo.exe+8533D8+(44*0)+20
24 - 0x18 - glowing red orbs    -   gnazo.exe+8533D8+(44*0)+24
28 - 0x1C - blue orbs           -   gnazo.exe+8533D8+(44*0)+28
32 - 0x20 - glowing blue orbs   -   gnazo.exe+8533D8+(44*0)+32
36 - 0x24 - green orbs          -   gnazo.exe+8533D8+(44*0)+36
40 - 0x28 - InPartyFlag         -   gnazo.exe+8533D8+(44*0)+40
41 - 0x29 - DeadOrAliveFlag     -   gnazo.exe+8533D8+(44*0)+41
```
