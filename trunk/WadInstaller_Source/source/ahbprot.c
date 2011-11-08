#include <gccore.h>
#include <ogc/machine/processor.h>
#include <stdio.h>
#include <string.h>
#include <unistd.h>

#include "ahbprot.h"

#define MEM_PROT 0xD8B420A


const u8 identify_check[] = { 0x28, 0x03, 0xD1, 0x23 };
const u8 identify_patch[] = { 0x00, 0x00 };
const u8 addticket_vers_check[] = { 0xD2, 0x01, 0x4E, 0x56 };
const u8 setuid_check[] = { 0xD1, 0x2A, 0x1C, 0x39 };
const u8 setuid_patch[] = { 0x46, 0xC0 };
const u8 isfs_perms_check[] = { 0x42, 0x8B, 0xD0, 0x01, 0x25, 0x66 };
const u8 e0_patch[] = { 0xE0 };
const u8 es_set_ahbprot_check[] = { 0x68, 0x5B, 0x22, 0xEC, 0x00, 0x52, 0x18, 0x9B, 0x68, 0x1B, 0x46, 0x98, 0x07, 0xDB };
const u8 es_set_ahbprot_patch[] = { 0x01 };

const u32 identify_check_size = sizeof(identify_check);
const u32 identify_patch_size = sizeof(identify_patch);
const u32 addticket_vers_check_size = sizeof(addticket_vers_check);
const u32 setuid_check_size = sizeof(setuid_check);
const u32 setuid_patch_size = sizeof(setuid_patch);
const u32 isfs_perms_check_size = sizeof(isfs_perms_check);
const u32 e0_patch_size = sizeof(e0_patch);
const u32 es_set_ahbprot_check_size = sizeof(es_set_ahbprot_check);
const u32 es_set_ahbprot_patch_size = sizeof(es_set_ahbprot_patch);

static u8 certs[0xA00] ATTRIBUTE_ALIGN(32);
static const char certs_fs[] ATTRIBUTE_ALIGN(32) = "/sys/cert.sys";

const u8 hash_old[] = { 0x20, 0x07, 0x4B, 0x0B };
const u8 hash_patch[] = { 0x00 };

int get_certs(void) {
    int fd, ret;
    fd = IOS_Open(certs_fs, 1);
    ret = IOS_Read(fd, certs, sizeof(certs));
    if (ret < sizeof(certs)) {
        ret = -1;
    } else {
        IOS_Close(fd);
    }
    return ret;
}

u32 apply_patch2(const char *name, const u8 *old, u32 old_size, const u8 *patch, u32 patch_size, u32 patch_offset) {
    u32 * end = (u32 *) 0x80003134;
    u8 *ptr = (u8 *) *end;
    u32 i, found = 0;
    u8 *start;
    
    while ((u32) ptr < (0x94000000 - old_size)) {
        if (!memcmp(ptr, old, old_size)) {
            found++;
            start = ptr + patch_offset;
            for (i = 0; i < patch_size; i++) {
                *(start + i) = patch[i];
            }
            ptr += patch_size;
            DCFlushRange((u8 *) (((u32) start) >> 5 << 5), (patch_size >> 5 << 5) + 64);
            ICInvalidateRange((u8 *)(((u32)start) >> 5 << 5), (patch_size >> 5 << 5) + 64);
            break;
        }
        ptr++;
    }
    return found;
}

u32 patchSetAHBPROT() {
    u32 count = 0;
	write16(MEM_PROT, 0);
	count += apply_patch2("Set AHBPROT patch", es_set_ahbprot_check, es_set_ahbprot_check_size, es_set_ahbprot_patch, es_set_ahbprot_patch_size, 25);
	write16(MEM_PROT, 1);
    return count;
}

u32 runtimePatchApply() {
    u32 count = 0;
    write16(MEM_PROT, 0);
    count += apply_patch2("New Trucha (may fail)", hash_old, sizeof(hash_old), hash_patch, sizeof(hash_patch), 1);
    count += apply_patch2("ES_Identify", identify_check, identify_check_size, identify_patch, identify_patch_size, 2);
    count += apply_patch2("NAND Permissions", isfs_perms_check, isfs_perms_check_size, e0_patch, e0_patch_size, 2);
    count += apply_patch2("Add ticket patch", addticket_vers_check, addticket_vers_check_size, e0_patch, e0_patch_size, 0);
    count += apply_patch2("ES_SetUID", setuid_check, setuid_check_size, setuid_patch, setuid_patch_size, 0);
    write16(MEM_PROT, 1);
    return count;
}