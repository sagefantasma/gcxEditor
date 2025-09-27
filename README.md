# Gcx Editor

A tool designed to allow users to edit .gcx files from MGS2 Master Collection Version.

## How do I use this?
1. Download a release or build the solution locally.
1. Run `GcxEditorGUI.exe`. 
1. Load a .gcx file from MGS2. 
1. After the .gcx and dictionary is loaded, the right-side of the main panel will be loaded with a JSON document representing the decoded contents of the selected .gcx file. You can use the list on the left-side of the main panel to navigate to references of each procedure in the json document - including calls to the procedure and the procedure's own declaration. 
1. Within the JSON document, you can modify any object or element thereof. 
1. Once you are finished modifying the JSON document, you can "export" it to a .gcx file containing the changes you made, assuming there are no invalid modifications. 
1. If the export completes with no errors, the .gcx file created you will have a valid .gcx that *should* be loadable by MGS2.

## What is this useful for?
- Reverse engineering
- Game script modding
- Memes

## Tutorials!
- [Basic Reverse Engineering](https://youtu.be/zo1hXALpmJM)
- [Basic Object Movement](https://youtu.be/4RraMLQ4hio)
- [Advanced Behavior Modding](https://youtu.be/UgAFV9LMOSs)

## Future Plans
- Addition of a visual-programming editing option as an alternative to JSON editing
- Script resource editing(see the MGS MC Mod Manager @ANTIBigBoss owns and maintains [here](https://github.com/ANTIBigBoss/MGS-MC-Mod-Manager-and-Tool) for stage resource editing)
- String editing
- Font editing
- MGS3 MC support?

## Please consider supporting this project on [Ko-Fi](https://ko-fi.com/sagefantasma)!
Donations are not expected or required, however I would be truly honored to earn your support!