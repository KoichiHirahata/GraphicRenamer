GraphicRenamer
=============
The file management tool that can handle JPEG files, PDF files, and endoscopy images.

##Description
This software will make patient's ID name folder and store files.  
The file name format is [ID]_[Date(YYYYMMDD)]_[sequential serial number].[extension]  
**++This software will move and change names of the files. Be sure to backup them.++**  
  
To view stored files, please use [PtGraViewer](https://github.com/KoichiHirahata/PtGraViewer).

##Requirements
Windows XP or later.  
.NET Flamework 4.0

##Usage
Fill in a blank box with patient's ID, and then drag-and-drop files into the right side of the window.  
You may handle more than one object at the same time, but they should be the same type (JPEG files or PDF files or endoscopy image folders).

###Gastrointestinal Endoscopy Images
The software can handle endoscopy system of Olympus and Fujifilm.  
**++Please be sure to backup before procedure.++**

####Olympus
Endoscopy system of Olympus store images in date name folders.  
The folder name format is YYMMDD.  
Drag-and-drop them to store.

####Fujifilm
Image folders stored in date name folders.  
The name format of date name folders is YYYYMMDD.  
Drag-and-drop image folders.(Not date name folders).

##Initial settings
1. Click "Settings".
2. Input folder you want to save files.  
If you want to share files with other computers, specify shared folder or NAS.
3. If you want to use Findings Editor's database server to indicate patient's name automatically, check the box of "Use database server of Findings Editor", and fill in the blanks of the window.
4. Click "Save" button.

##Limitations of the software
If you use a drive formatted with FAT32, you may store 65534 patient's files.  
We recommend using a drive that formatted with NTFS.  
You may store 999 series of files a day every patient.

##Licence
Licensed under the GPL v3

##Auther
[Koichi Hirahata](https://github.com/KoichiHirahata)
