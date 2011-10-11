/------------------------------------------------------------------------------>
                                  Sharpii 1.3
       <---------------------------------------------------------------->
                               An app by person66
                 libWiiSharp.dll by leathl (mod by scooby74029)                         
/------------------------------------------------------------------------------>



/----DESCRIPTION
/------------------------------>

Sharpii is a command line app that I made, which uses leathl's 
libWiiSharp.dll to perform tasks such as:
         - Pack, unpack, or edit .wad files
         - Pack, and unpack U8 archives
         - Patch IOS .wad files with various patches
		 - Download files from NUS
         - Convert a .wav file to .bns, and vice versa
         - Convert an image file to a .tpl, and vice versa
         - Send a .dol to the Homebrew Channel over Wi-Fi


/----USAGE
/------------------------------>

I won't go in to detail here, but to see all the commands, just start 
up the command prompt, navigate to the folder containing Sharpii, and type:

                                    Sharpii.exe -h

OR, if you want help with a specific function use

                             Sharpii.exe [function] -h

Where [function] would obviously be replaced with the function you want
help with.


/----NOTES
/------------------------------>

NUS Downloading:
/------------------>
	When downloading single contents from NUS (using the -s argument) make
	sure you have both the path, and the file name when specifying the output.
	For example, if the output is set to '.\hello.app' then the file will be
	saved as 'hello.app' in the current directory. However, if the output is 
	set to 'hello.app' you will get an error.
	
	Also note that When Downloading single contents, it will only save the
	decrypted file.
	
WAD Editing:
/------------------>
	When changing the type of WAD (using the -type argument) some of the types
	may not work, as they have not all been tested. Here is a list of what the
	different types are:
		- Channel: Regular channel WAD, nothing special
		- DLC: WAD for game DLC (downloaded game content)
		- GameChannel: Channels such as the Wii Fit or Mario Kart channels
		- HiddenChannels: A hidden channel, it wont show up on the Wii Menu
		- SystemChannels: Channels such as the Mii or Shopping channels
		- SystemTitles: Stuff like the System Menu and boot2 (but not IOSs)
		
	For more details see http://wiibrew.org/wiki/Title_database


/----SOURCE
/------------------------------>

The source for Sharpii is available at: sharpii.googlecode.com


/----CREDITS
/------------------------------>

Sharpii uses scooby74029's mod of libWiiSharp.dll by leathl, and 
it borrows some code from some of the examples included with libWiiSharp.

libWiiSharp can be found at: libwiisharp.googlecode.com


I would also like to thank XFlak and JoostinOnline for doing a bit of beta 
testing for me. Thanks!


/----LICENSE
/------------------------------>

Sharpii is released under the terms of the GNU General Public License v3.
See "LICENSE.txt" for more information.


/----CHANGELOG
/------------------------------>

1.3
  - Added the ability to copy parts of one WAD to a different
    WAD (either the banner, the icon, the sound, or the dol)
  - Added the ability to download just a single content from NUS
  - Code cleanup and bug fixes
  - Sharpii can now find JoostinOnline a girlfriend! :P
1.2
  - Added version patch support for IOS patching
  - Switched to scooby74029's mod of libWiiSharp 
  - Bug fixes
1.1
  - Added support for NUS downloading
1.0
  - Initial release