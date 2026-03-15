# Sims 3 Package Reducer (S3PR) by OhRudi
Software to easily reduce, compress and decompress Package-Files for the Sims 3

## Overview
- This software **reduces the size of Package-Files drastically**, very usefull if you have tons of CC
- It removes thumbnails (THUM) and icons (ICON) resources from Package-Files and compresses or decompresses them
- Thumbnails are safe to remove, the game regenerates them (see image explenation)
- Icons are safe to remove, the game doesn't use them, nor does CC-Magic
- __Compressing reduces__ the Package-File size drastically and but it's no performance gain for your game, because your game has to decompresses them while playing
- __Decompressing expands__ the Package-File size by three times. It's a performance gain while playing, cause your game needs them decompressed anyway
- I'd recommend **compressing your individual CC files**, but **decompressing the merged CC files**, such as the builds from CC-Magic
- ***DO NOT*** compress mods (like NRaas and other Script-Mods), only compress CC
- Works only for Package-Files, *not for Sims3Pack-Files*
- Works only for Windows (did test for Windows 10 & 11), not for Mac, did not test on Linux

## How to compile
```powershell
# WITHIN THE S3PR PROJECT ROOT

# pull tags from github
git pull origin main --tags --force

# get latest git tag/release
$versionNumber = (git tag --sort=-v:refname | Select-Object -First 1) -replace '^(v|V|version|VERSION|Version)', ''

# compile into a single file
# paste latest git tag (version number) into config
# this version number is then displayed in the UI
# the compiled exe is under: S3PR_GUI\bin\Release\net10.0-windows\win-x64\publish
dotnet publish .\S3PR_GUI\S3PR_GUI.csproj -c Release -r win-x64 --self-contained true /p:PublishSingleFile=true /p:AssemblyVersion="$versionNumber" /p:Version="$versionNumber" /p:FileVersion="$versionNumber" /p:InformationalVersion="$versionNumber"
```

## How do I use the Console Application?
**Important Notice:** Everything is the same, but if you're into automation, you can use S3PR via the console.
```
Usage:
S3PR_Console.exe [options] <paths to folders or files...>

Options:
-r, --search-recursive     Search directories recursively when looking for Package-Files.

-t, --remove-thumbnail     Remove thumbnail resources from Package-Files.

-i, --remove-icon          Remove icon resources from Package-Files.

-c, --compress-file        Compress Package-Files.

-d, --decompress-file      Decompress Package-Files.

-s, --silent               No Console Output.

-h, --help                 Show this help message and exit.

Arguments:
<paths...>                 One or more files or folders to process.
```

## Further Explenations
Further explenations and releases of this software you can find on [ModTheSims](https://modthesims.info/m/10288520), [Simblr](https://simblr.cc/user/326-ohrudi/), or [Tumblr](https://www.tumblr.com/ohrudi)

## Technical Details
- It's published under the GNU General Public License (GPL-3.0)
- Feel free to change fork the code and change it any way you like, this source code meant to be public, free and open
- I used __Visual Studio 2026 with .NET 10.0__ and WinForms API to create the GUI

## Big thanks to ...
- [Sims 3 Easy Extractor & Merger Tools](https://simblr.cc/ts3/mod/7154-sims-3-easy-extractor-merger-tools.html) by phantom99 (their project and open source code inspired me to do this)
- [Sims 3 Package Interface](https://sourceforge.net/projects/s3pi/) by Peter L Jones
- [Sims 3 Recompressor (S3RC) Tool](http://www.moreawesomethanyou.com/smf/index.php/topic,15129.0.html) by J. M. Pescado
- [Pixlr.com](https://pixlr.com/) which I always use for free image editing
- [Seasons Coat with Buttons for teens](https://simblr.cc/ts3/mod/6669-seasons-coat-with-buttons-for-teens.html) by sweetdevil, which I used for example pictures
- [Set of hairstyles Skysims](https://simblr.cc/ts3/mod/6714-set-of-hairstyles-skysims.html) by Agnelid, which I used for example pictures
- AND to this awesome and still alive community of simmers ^^
