﻿
public static class DamageFormulas {

    [DamageFormula]
    public static float Slash(OldContext context, float baseValue) {
        return 10f;
    }

    [DamageFormula]
    public static float ShadowSlash(OldContext context, float baseValue) {
        return 11;
    }

}