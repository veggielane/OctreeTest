﻿using OpenTK.Graphics.OpenGL;
using Veg.OpenTK.Shaders;

namespace OctreeTest
{
    public class BasicShaderProgram : BaseShaderProgram
    {
        public BasicShaderProgram(CameraUBO ubo)
        {
            CompileShader(ShaderType.VertexShader, @"#version 400
precision highp float; 

layout(std140) uniform Camera {
    mat4 MVP;
    mat4 Model;
    mat4 View;
    mat4 Projection;
    mat4 NormalMatrix;
};
layout (location = 0) in vec3 vert_position; 
layout (location = 1) in vec3 vert_normal; 
layout (location = 2) in vec4 vert_colour; 
out vec4 col;

void main(void) 
{ 
    gl_Position = (MVP) * vec4(vert_position, 1); 
    col = vert_colour;
}");
            CompileShader(ShaderType.FragmentShader, @"#version 400
in vec4 col;
layout( location = 0 ) out vec4 FragColor;
void main() {
    FragColor = col;
}");
            Link();
            ubo.BindToShaderProgram(this);
        }
    }
}