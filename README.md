# Ion Lang Compiler

This project is a compiler built using C# and .Net Core for the Ion Language. The Ion language is one which sits like a layer above the C language and abstracts away some of C's more annoying and archaic features. It transpiles into C99 source which can then be further compiled using a standard compiler such as GCC/VisualC. Although Ion does not stray far from its C roots, it does implement many modern features such as type inference, basic type reflection and code packaging. Like C it is procedurally based with features such as structs/enums and functions being the basic objects with which one works. Unlike C it has no need for header files, function definitions or macros.

**Ion is part of something much bigger.**

The Ion language was designed entirely by Per Vognsen. It is the central piece to the Bitwise educational project he created which aims to build an entire software and hardware stack for the RISC V CPU. The bitwise project can be found at the following links.

Handmade: https://bitwise.handmade.network/

YouTube: https://www.youtube.com/pervognsen

GitHub: https://github.com/pervognsen/bitwise

Discord: https://discord.gg/7TSA6ZF

**Why I created IonDotNet.**

The Ion language was originally created in C with the intention of then creating a self-compiling compiler. I wanted to follow along with Per when he created the streams but didn’t have the time. Instead I followed at my own time & pace. I feel the best way to accomplish this is not to just copy code in the language used. This leads to boredom and no real understanding is gained. I choose to transcribe the code into a second similar but different language. This forces you to read and grasp the code which leads to one gaining a much better understanding of two languages at the same time thus accelerating ones learning rate. I choose C# as the destination language since it shares its origin’s with C and although is a much higher-level language, it still allows direct access to memory using pointers and structs.

There are two versions here. The first is a direct port of the code to the C#. The code is very similar both in style and substance. In many cases the code is identical. I used C's naming and styling conventions to make going between this and Pers' code as easy as possible. Calls to the .Net framework is kept to a minimum. I wanted to see how fast could Native C# code run when compared to C.
The second project called IonManaged uses .Nets containers and standard library in place of custom ones created in the orignial port. This project is suitable for using in tooling and as a library since memory is managed by the .Net Garbage Collector.
