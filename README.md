GraphicRenamer
=============
This software will make patient's ID name folder and store JPEG files and PDF files.
Files will named by rule.
The file name format is [ID]_[Date(YYYYMMDD)]_[sequential serial number].[extension]
**++This software will move and change names of the files. Be sure to backup them.++**

To show stored files, please use [PtGraViewer](https://github.com/KoichiHirahata/PtGraViewer).

##Initial settings
1. Click "Settings".
2. Input folder you want to save files.
If you want to share files with other computers, specify shared folder or NAS.
3. If you want to use Findings Editor's database server to indicate patient's name automatically, check the box of "Use database server of Findings Editor", and fill in the blanks of the window.
4. Click "Save" button.

##How to use
Fill in a blank box with patient's ID, then drag-and-drop files into right side of the window.
You may handle more than one file at the same time, but they should be same type file (JPEG or PDF).

###Gastrointestinal Endoscopy Images
You can store gastrointestinal endoscopy images with this software.
The software can deal with endoscopy system of Olympus and Fujifilm.
**++Please be sure to backup before this procedure.++**

####Olympus
Endoscopy system of Olympus store images in date name folders.
The folder name format is YYMMDD.
Drag-and-drop them to store.

####Fujifilm
Image folders were stored in date name foleders.
The name format of date name folders is YYYYMMDD.
Drag-and-drop image folders.(Not date name folders).

##Limitations of the software
If you use drive formated with FAT32, you may store 65534 patient's files.
We recommend to use a drive which formatted with NTFS.
You may store 999 series of files a day every patient.

##Licence
GPL v3