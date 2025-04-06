sampler uImage0 : register(s0);
sampler uImage1 : register(s1);//Broken Screen
sampler uImage2 : register(s2);
sampler uImage3 : register(s3);
float3 uColor; 
float3 uSecondaryColor;
float2 uScreenResolution;
float2 uScreenPosition; 
float2 uTargetPosition;
float2 uDirection;
float uOpacity;
float uTime;
float uIntensity;
float uProgress;
float2 uImageSize1;
float2 uImageSize2;
float2 uImageSize3;
float2 uImageOffset;
float uSaturation;
float4 uSourceRect;
float2 uZoom;

float4 Universe(float2 coords : TEXCOORD0) : COLOR0
{
    float4 color = tex2D(uImage0, coords);
    float2 range = float2(1 / uScreenResolution.x, 1 / uScreenResolution.y);
    float4 le = tex2D(uImage0, float2(coords.x - range.x, coords.y));
    float4 ri = tex2D(uImage0, float2(coords.x + range.x, coords.y));
    float4 up = tex2D(uImage0, float2(coords.x, coords.y + range.y));
    float4 dw = tex2D(uImage0, float2(coords.x, coords.y - range.y));

    if (color.x == uColor.x && color.y == uColor.y && color.z == uColor.z)
    {
        if (le.x != uColor.x || ri.x != uColor.x || up.x != uColor.x || dw.x != uColor.x)
            return color;
        float2 offset = uScreenPosition / uScreenResolution;
        float2 size = uImageSize1 / uScreenResolution;
        return tex2D(uImage1, (offset + coords) * size + uIntensity);
    }   
    return color;
}

technique Technique1
{
    pass FragmentScreen
    {
        PixelShader = compile ps_2_0 Universe();
    }
}