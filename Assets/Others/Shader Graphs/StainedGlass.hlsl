#ifndef STAINED_GLASS_INCLUDED
#define STAINED_GLASS_INCLUDED

float2 StainedGlass_Hash2(float2 p)
{
    p = float2(dot(p, float2(127.1, 311.7)), dot(p, float2(269.5, 183.3)));
    return frac(sin(p) * 43758.5453);
}

float3 StainedGlass_Hue(float h)
{
    return saturate(abs(frac(h + float3(0.0, 2.0 / 3.0, 1.0 / 3.0)) * 6.0 - 3.0) - 1.0);
}

// Seeds orbit inside their own cell (kept within 0.05..0.95) so the 3x3 / 5x5 searches stay valid.
float2 StainedGlass_Seed(float2 cellId, float time)
{
    float2 h = StainedGlass_Hash2(cellId);
    return 0.5 + 0.45 * sin(time * (0.6 + h.yx * 0.8) + 6.2831 * h);
}

// Voronoi panes: per-pane tint and refraction, bevelled edges and lead came between panes.
void StainedGlass_float(float2 UV, float CellScale, float LeadWidth, float Refraction, float TintStrength, float Speed,
    out float2 RefractedUV, out float3 Tint, out float Lead, out float Glint)
{
    float time = _Time.y * Speed;
    float aspect = _ScreenParams.x / _ScreenParams.y;
    float2 p = float2(UV.x * aspect, UV.y) * CellScale;
    float2 cell = floor(p);
    float2 f = frac(p);

    float2 nearestGrid = 0;
    float2 nearestOffset = 0;
    float nearestDist = 8.0;
    for (int j = -1; j <= 1; j++)
    {
        for (int i = -1; i <= 1; i++)
        {
            float2 g = float2(i, j);
            float2 r = g + StainedGlass_Seed(cell + g, time) - f;
            float d = dot(r, r);
            if (d < nearestDist)
            {
                nearestDist = d;
                nearestOffset = r;
                nearestGrid = g;
            }
        }
    }

    float edgeDist = 8.0;
    for (int y = -2; y <= 2; y++)
    {
        for (int x = -2; x <= 2; x++)
        {
            float2 g = nearestGrid + float2(x, y);
            float2 r = g + StainedGlass_Seed(cell + g, time) - f;
            float2 diff = r - nearestOffset;
            if (dot(diff, diff) > 1e-5)
                edgeDist = min(edgeDist, dot(0.5 * (nearestOffset + r), normalize(diff)));
        }
    }

    float2 paneRand = StainedGlass_Hash2(cell + nearestGrid + 17.31);
    float2 toCenter = normalize(nearestOffset + 1e-5);

    float bevel = saturate(1.0 - (edgeDist - LeadWidth) / (LeadWidth * 3.0));
    bevel *= bevel;

    float2 offset = (paneRand - 0.5) * Refraction + toCenter * bevel * Refraction * 1.5;
    offset.x /= aspect;
    RefractedUV = saturate(UV + offset);

    float3 paneColor = lerp(1.0, StainedGlass_Hue(paneRand.x), lerp(0.45, 0.9, paneRand.y));
    Tint = lerp(1.0, paneColor, TintStrength);

    float aa = max(fwidth(edgeDist), 1e-4);
    Lead = 1.0 - smoothstep(LeadWidth - aa, LeadWidth + aa, edgeDist);

    float2 lightDir = normalize(float2(-0.6, 0.8));
    float facing = saturate(dot(-toCenter, lightDir));
    Glint = (pow(facing, 3.0) * bevel * 0.35 + paneRand.y * 0.03) * (1.0 - Lead);
}

void StainedGlassComposite_float(float3 Scene, float3 Tint, float Lead, float Glint, float3 LeadColor, out float3 Out)
{
    float3 glass = Scene * Tint + Glint;
    Out = lerp(glass, LeadColor, Lead);
}

#endif
