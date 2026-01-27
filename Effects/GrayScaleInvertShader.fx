sampler uImage0 : register(s0);
sampler uImage1 : register(s1);
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

float4 GrayScaleShader(float2 coords : TEXCOORD0) : COLOR0
{
    float4 color = tex2D(uImage0, coords);
    float ave = 0.299 * color.r + 0.587 * color.g + 0.114 * color.b;
    color.rgb = lerp(color.rgb, float3(ave, ave, ave), uProgress);
    return color;
}

float4 InvertShader(float2 coords : TEXCOORD0) : COLOR0
{
    float4 color = tex2D(uImage0, coords);
    float3 inv = float3(1.0 - color.r, 1.0 - color.g, 1.0 - color.b);
    color.rgb = lerp(color.rgb, inv, uIntensity);
    return color;
}

technique Technique1
{
    pass GrayScale
    {
        PixelShader = compile ps_2_0 GrayScaleShader();
    }
    pass Invert
    {
        PixelShader = compile ps_2_0 InvertShader();
    }
}