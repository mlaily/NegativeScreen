
read -p "Please remove any previous 'release' folder before continuing..."

mkdir release

cp NegativeScreen/bin/Release/NegativeScreen.exe release/NegativeScreenX86.exe
cp NegativeScreen/bin/x64/Release/NegativeScreen.exe release/NegativeScreen.exe
cp CHANGELOG.txt release/
cp LICENSE.txt release/
cp README.md release/

zip -j Binary.zip release/*

read -p "Now generating the chocolatey package. Press enter to proceed..."
choco.exe pack Chocolatey/negativescreen.nuspec --out=Chocolatey/
