sampler uImage0 : register(s0);
sampler uImage1 : register(s1);
sampler uImage2 : register(s2);
sampler uImage3 : register(s3);
float3 uColor; // r = radius
float3 uSecondaryColor;
float2 uScreenResolution;
float2 uScreenPosition; 
float2 uTargetPosition; // Swirl point
float2 uDirection;
float uOpacity;
float uTime;
float uIntensity;
float uProgress; // SwirlBigness
float2 uImageSize1;
float2 uImageSize2;
float2 uImageSize3;
float2 uImageOffset;
float uSaturation;
float4 uSourceRect;
float2 uZoom;

float4 Swirl(float2 coords : TEXCOORD0) : COLOR0
{
    float effectradius = uColor.r;
    float effectangle = uProgress * 3.14159;
    
    float2 center = (uTargetPosition - uScreenPosition) / uScreenResolution;
    center = center == float2(0, 0) ? float2(.5, .5) : center;
    
    float2 uv = coords - center;
    
    float len = length(uv * float2(uScreenResolution.x / uScreenResolution.y, 1));
    float angle = atan2(uv.y, uv.x) + effectangle * smoothstep(effectradius, 0, len);
    float radius = length(uv);
    
    coords = float2(radius * cos(angle), radius * sin(angle)) + center;
    
    return tex2D(uImage0, coords);
}

technique Technique1
{
    pass SwirlDistortion
    {
        PixelShader = compile ps_2_0 Swirl();
    }
}