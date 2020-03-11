using System;
using UnityEngine;

public class Noise 
{
    //private Vector2 resolution;
    //private float time;
    private static float lacunarity = 4.0F; //2.0
    private static float gain = 0.5F; //0.5
    private static int octaves = 10; //6
    
    private static Vector2 vec2(float x, float y)
    {
        return new Vector2(x, y);
    }
    
    private static float fract(float x)
    {
        return x - (float)Math.Floor(x);
    }
    
    private static float random(Vector2 st)
    {
        return fract((float)Math.Sin(Vector2.Dot(st, new Vector2(12.9898F, 78.233F))) * 43758.5453123F);
    }
    
    private static float noise(Vector2 st)
    {
        Vector2 i = vec2((float)Math.Floor(st.x), (float)Math.Floor(st.y));
        float tmp = fract(st.x);
        Vector2 f = new Vector2(tmp, tmp);
        
        float a = random(i);
        float b = random(i + vec2(1.0F, 0.0F));
        float c = random(i + vec2(0.0F, 1.0F));
        float d = random(i + vec2(1.0F, 1.0F));
        
        Vector2 u = f * f * (new Vector2(3.0F, 3.0F) - 2.0F * f);
        
        return UnityEngine.Mathf.Lerp(a, b, u.x) +
            (c - a) * u.y * (1.0F - u.x) +
            (d - b) * u.x * u.y;
    }
    
    public static float fbm(float x, float y)
    {
        Vector2 st = new Vector2(x, y);
        
        float value = 0.0F;
        float amplitude = 0.5F;
        float frequency = 0.0F;
        
        for (int i = 0; i < octaves; i++) 
        {
            value += amplitude * noise(st);
            st *= lacunarity;
            amplitude *= gain;
        }
        
        return value;
    }
}