﻿#version 330 core
layout(location = 0) in vec3 Position;
layout(location = 1) in vec2 TexCoord;

out vec2 fragTexCoord;
uniform mat4 model;
uniform mat4 view;
uniform mat4 projection;

void main()
{
	gl_Position = projection * view * model * vec4(Position, 1.0f);
	fragTexCoord = vec2(TexCoord.x, TexCoord.y);
}