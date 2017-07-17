GraphicRenamer
=============
File management tool for JPEG files, PDF files, and endoscopic images.

##Description
Software creates patient ID name folder and automatically stores files.  
Format: [ID]_[Date(YYYYMMDD)]_[sequential serial number].[extension]  
(e.g. 1000_20140302_xxx.jpg)  
**As software will move and change file names, please back up files prior.**  
  
To view stored files, please use [PtGraViewer](https://github.com/KoichiHirahata/PtGraViewer).

##Requirements
Windows 7 or later.  
.NET Flamework 4.5.2 or later

##Usage
Fill in patient's ID, then drag-and-drop files into the right side of the window.  
You may simultaneously drag-and-drop multiple files (identical extension types) or folders.

###Gastrointestinal Endoscopy Images
Software is only compatible with Olympus and Fujifilm digital images.  
**Please be sure to backup before transfer.**

####Tested Endoscopy Systems
[Olympus]
EVIS LUCELA ELITE CV-290

[Fujifilm]
Advancia HD(VP-4450HD)
Sapientia(VP-4400)

####Olympus
Endoscopy system's images are stored in date name folders (YYMMDD).
Drag-and-drop date name folders (Not image folders).

####Fujifilm
Endoscopic image folders are stored in date-name folders (YYYYMMDD).  
Drag-and-drop image folders (Not date name folders).

##Initial settings
1. Click "Settings".  
2. Select folder for saved files. To share files with other computers, specify shared folder or NAS.  
3. If you want to use Findings Editor's database server to indicate patient's name automatically, check "Use database server of Findings Editor" box and fill the blanks.  
4. Click "Save".

##Software Limitations
If you use a drive formatted with FAT32, you may store 65,534 patient's files.  
We recommend using a drive formatted with NTFS.  
You may store 999 series/collections per patient per day.

##Licence
Licensed under the GPL v3

##Auther
[Koichi Hirahata](https://github.com/KoichiHirahata)
