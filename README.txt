# NegativeScreen #

http://arcanesanctum.net/negativescreen

***

## Description

NegativeScreen's main goal is to support your poor tearful eyes when enjoying the bright white interweb in a dark room.
This task is joyfully achieved by inverting the colors of your screen.

Unlike the Windows Magnifier, which is also capable of such color inversion,
NegativeScreen was specifically designed to be easy and convenient to use.

It comes with a minimal graphic interface in the form of a system tray icon with a context menu,
but don't worry, this only makes it easier to use!


## Features

Invert screen's colors.

Additionally, many color effects can be applied.
For example, different inversion modes, including "smart" modes,
swapping blacks and whites while keeping colors (about) the sames.

You can now configure the color effects manually via a configuration file.
You can also configure the hot keys for every actions, using the same configuration file.

A basic web api is part of NegativeScreen >= 2.5
It is disabled by default. When enabled, it listens by default on port 8990, localhost only.
See the configuration file to enable the api or change the listening uri...

All commands must be sent with the POST http method.
The following commands are implemented:
- TOGGLE
- ENABLE
- DISABLE
- SET "Color effect name" (without the quotes)

Any request sent with a method other than POST will not be interpreted,
and a response containing the application version will be sent.


## Requirements

NegativeScreen < 2.0 needs at least Windows Vista to run.

Versions 2.0+ need at least Windows 7.

Both run on Windows 8 or superior.

Graphic acceleration (Aero) must be enabled.


## Default controls

-Press Win+Alt+H to Halt the program immediately
-Press Win+Alt+N to toggle color inversion (mnemonic: Night vision :))

-Press Win+Alt+F1-to-F10 to switch between inversion modes:
	* F1: standard inversion
	* F2: smart inversion1 - theoretical optimal transformation (but ugly desaturated pure colors)
	* F3: smart inversion2 - high saturation, good pure colors
	* F4: smart inversion3 - overall desaturated, yellows and blues plain bad. actually relaxing and very usable
	* F5: smart inversion4 - high saturation. yellows and blues  plain bad. actually quite readable
	* F6: smart inversion5 - not so readable. good colors. (CMY colors a bit desaturated, still more saturated than normal)
	* F7: negative sepia
	* F8: negative gray scale
	* F9: negative red
	* F10: red
	* F11: grayscale


## Configuration file

A default configuration file comes with the binary release.

Should something go wrong (bad hot key...), you can simply delete the configuration file,
the internal default configuration will be used.

If the configuration file is missing, you can use the "Edit Configuration" menu to regenerate the default one.

Syntax: see in the configuration file...


***

Many thanks to Tom MacLeod who gave me the idea for the "smart" inversion mode :)


Enjoy!