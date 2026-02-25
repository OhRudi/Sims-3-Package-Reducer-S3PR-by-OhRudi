# Sims 3 Package Reducer (S3PR) by OhRudi
Software to easily reduce and compress Package-Files

## Overview
- This software **reduces the size of Package-Files drastically**, very usefull if you have tons of CC
- It removes thumbnails (THUM) and icons (ICON) resources from Package-Files and compresses them
- Thumbnails are safe to remove, the game regenerates them (see image explenation)
- Icons are safe to remove, the game doesn't use them, nor does CC-Magic
- Compressing reduces mod size drastically and lets CC load faster into the game
- Works only for Package-Files, *not for Sims3Pack-Files*
- Works only for Windows (did test for Windows 10 & 11), not for Mac, did not test on Linux

## How to compile
```powershell
# pull tags from github
git pull origin main --tags --force

# get latest git tag/release
$versionNumber = (git tag --sort=-v:refname | Select-Object -First 1) -replace '^(v|V|version|VERSION|Version)', ''

# compile into a single file
# paste latest git tag (version number) into config
# this version number is then displayed in the UI
# the compiled exe is under: S3PR_GUI\bin\Release\net10.0-windows\win-x64\publish
dotnet publish -c Release -r win-x64 --self-contained true /p:PublishSingleFile=true /p:AssemblyVersion="$versionNumber" /p:Version="$versionNumber" /p:FileVersion="$versionNumber" /p:InformationalVersion="$versionNumber"
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
- AND to this awesome and still alive community of simmers ^^
