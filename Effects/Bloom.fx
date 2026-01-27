sampler uImage0 : register(s0);

float2 uScreenResolution;
float uRange;
float uIntensity;
float uThreshold;
float weight[5] = { 0.227027, 0.1945946, 0.1216216, 0.054054, 0.016216 };

float4 BloomShader(float2 coords : TEXCOORD0) : COLOR0
{
    float4 color = tex2D(uImage0, coords);
    float brightness = dot(color.rgb, float3(0.21, 0.71, 0.07));
    if (brightness > uThreshold)
        return color;
    else
        return float4(0, 0, 0, 0);
}

float4 GBlurHShader(float2 coords : TEXCOORD0) : COLOR0
{
    float offset = uRange / uScreenResolution;
    float4 result = tex2D(uImage0, coords) * weight[0];
    for (int i = 1; i < 5; i++)
    {
        result += tex2D(uImage0, coords + float2(offset * i, 0)) * weight[i];
        result += tex2D(uImage0, coords - float2(offset * i, 0)) * weight[i];
    }
    return result * uIntensity;
}

float4 GBlurVShader(float2 coords : TEXCOORD0) : COLOR0
{
    float offset = 1.0 / uScreenResolution;
    float4 result = tex2D(uImage0, coords) * weight[0];
    for (int i = 1; i < 5; i++)
    {
        result += tex2D(uImage0, coords + float2(0, offset * i)) * weight[i];
        result += tex2D(uImage0, coords - float2(0, offset * i)) * weight[i];
    }
    return result * uIntensity;
}

technique Technique1
{
    pass Bloom
    {
        PixelShader = compile ps_2_0 BloomShader();
    }
    pass GBlurH
    {
        PixelShader = compile ps_2_0 GBlurHShader();
    }
    pass GBlurV
    {
        PixelShader = compile ps_2_0 GBlurVShader();
    }
}