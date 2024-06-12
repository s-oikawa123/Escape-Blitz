Shader "Unlit/MirrorMask"
{
    Properties  
    {
        _MainTex("Texture", 2D) = "white"{}  
        _Stencil("Stencil", int) = 1
    }

    SubShader  
    {
        Tags {"Queue"="Geometry"}  
        Pass  
        {
            Stencil {  
                Ref [_Stencil]
                Comp Always  
                Pass Replace  
            }
            CGPROGRAM  
            sampler2D _MainTex;  
            #pragma vertex vert_img  
            #pragma fragment frag  
            #include "UnityCG.cginc"  

            fixed4 frag (v2f_img i) : SV_Target 
            {
                fixed4 c = tex2D(_MainTex, i.uv);  
                return c;  
            }
            ENDCG  
        }
    }
}
