#ifndef _RUNTIMEIOSPATCH_H_
#define _RUNTIMEIOSPATCH_H_

#ifdef __cplusplus
extern "C" {
#endif

#define HAVE_AHBPROT ((*(vu32*)0xcd800064 == 0xFFFFFFFF) ? 1 : 0)


int get_certs(void);
int check_fakesig(void);

extern const u8 identify_check[];
extern const u8 identify_patch[];
extern const u8 addticket_vers_check[];
extern const u8 setuid_check[];
extern const u8 setuid_patch[];
extern const u8 isfs_perms_check[];
extern const u8 e0_patch[];
extern const u8 es_set_ahbprot_check[];
extern const u8 es_set_ahbprot_patch[];

extern const u32 identify_check_size;
extern const u32 identify_patch_size;
extern const u32 addticket_vers_check_size;
extern const u32 setuid_check_size;
extern const u32 setuid_patch_size;
extern const u32 isfs_perms_check_size;
extern const u32 e0_patch_size;
extern const u32 es_set_ahbprot_check_size;
extern const u32 es_set_ahbprot_patch_size;

u32 patchSetAHBPROT();
u32 runtimePatchApply();

u32 PrintResult(u32 successful);
void ApplyingPatch(const char* which);

#ifdef __cplusplus
}
#endif

#endif
