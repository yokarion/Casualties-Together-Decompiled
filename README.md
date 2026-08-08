# Casualties-Together-Decompiled

Decompiled source mirror of **KrokMP**/**Casualties Together**, the multiplayer mod for Casualties:
Unknown, published at [creaturefeaturelarry/casualties-together](https://github.com/creaturefeaturelarry/casualties-together).

Everything under [decompiled](/decompiled) is generated automatically from the released DLLs
by [dotnet-autodecompiler](https://github.com/Yokarion/dotnet-autodecompiler)
and is overwritten on every update. Do not edit it by hand and do not open pull
requests against it.

This exists so the mod's code can be read and diffed between releases. It is a
reconstruction produced by a decompiler, not the author's original source, so
local variable names and compiler-generated members will not match what the
author wrote.

Closed Source sucks

## What is mirrored

The mod ships as a single zip laid out for dropping into the game directory.
Only the author's own assemblies are decompiled here:

| Assembly                   | What it is                                  |
| -------------------------- | ------------------------------------------- |
| `KrokoshaCasualtiesMP.dll` | The mod itself                              |
| `Multiupdater.dll`         | Update checker shipped with the mod         |
| `autoupdater_patcher.dll`  | BepInEx patcher that bootstraps the updater |

The release archive also contains `LiteNetLib.dll`, `Steamworks.NET.dll` and
`OpusSharp.Core.dll`, which are third party libraries by other authors, plus the
native `opus.dll` and `steam_api64.dll`. Those are deliberately not mirrored.

`decompiled/.autodecompiler.json` records which upstream release each mirror was
produced from, including the SHA-256 of every DLL.
