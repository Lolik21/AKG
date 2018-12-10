#version 330 core
layout(location = 0) in vec3 Position;
layout(location = 1) in vec3 Color;

out vec3 fragColor;
uniform mat4 transform;

void main()
{
	fragColor = Color;
	gl_Position =  transform * vec4(Position, 1.0f);
}