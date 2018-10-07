// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Unlit/Pixelization Shader"
{
    Properties 
    {
        _MainTex ("Base (RGB) Trans (A)", 2D) = "white" {}
        _PixelSize ("Pixel Size", Range(0, 1.0)) = 100
    }
     
    SubShader
    {
        Pass
        {           
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
             
            #include "UnityCG.cginc"
             
            sampler2D _MainTex;
            fixed _PixelSize;
             
            struct appdata
            {
                fixed4 vertex : POSITION;
                fixed2 uv : TEXCOORD0;
            };
             
            struct v2f
            {
                fixed4 vertex : SV_POSITION;
                fixed2 uv : TEXCOORD0;
            };
             
            v2f vert(appdata v)
            {
                v2f o;
                o.uv = v.uv;
                o.vertex = UnityObjectToClipPos(v.vertex);
                 
                return o;
            }
             
            fixed4 frag(v2f i) : COLOR
            {
                fixed2 uv = i.uv;
                 
                if(_PixelSize != 0)
                {               
                    uv = fixed2((int)(uv.x / _PixelSize), (int)(uv.y / _PixelSize)) * _PixelSize;
                }
                 
                fixed4 col = tex2D(_MainTex, uv);
                 
                return col;
            }
            ENDCG
        }
    }
}