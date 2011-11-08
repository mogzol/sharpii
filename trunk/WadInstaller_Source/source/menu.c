#include <stdio.h>
#include <stdlib.h>
#include <string.h>
#include <malloc.h>
#include <ogcsys.h>
#include <unistd.h>

#include "fat.h"
#include "restart.h"
#include "title.h"
#include "utils.h"
#include "video.h"
#include "wad.h"
#include "wpad.h"
#include "ahbprot.h"

/* Constants */
#define CIOS_VERSION		249


void LoadSelectedIOS() 
{
	u8 selectedIOS = Wad_SelectIOS();
	s32 ret;
	
	if (selectedIOS == 0) {
		if (HAVE_AHBPROT) {
			ret = patchSetAHBPROT();
			if (ret > 0) {
				ret = runtimePatchApply();
				if (ret > 0) {
					printf("\nUsing AHBPROT\n");
					return;
				}
			} 
			printf("\nAHBPROT FAILED!\n");
		}
	}
	
	ret = IOS_ReloadIOS(selectedIOS);
	if (ret<0) 
	{
		printf("\nUsing default IOS\n");
	} else 
	{
		printf("\nUsing selected IOS %d\n", selectedIOS);
	}	
}

void Menu_Loop(void)
{	
	Wad_InstallFromMemory();
}
