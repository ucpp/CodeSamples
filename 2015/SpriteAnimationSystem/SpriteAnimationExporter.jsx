/*
 * Sprite Animation Exporter ver.0.9.0
 */

/*
Скрипт экспортирует анимационные кадры и записывает файл с именем кадра, 
координатами, относительно якоря, и временем проигрывания.
Якорь - слой с именем "anchor" и минимальным изображением в один пиксель. 
По умолчанию центром является центр документа.

Имя файла содержит в себе имя поддиректорий имя объекта анимации и имя анимации,
разделенные символоми нижнего подчеркивания '_', например: Animation_NPC_enemy1_idle.psd.
Такой файл будет экспортирован в папку "Animation/NPC/enemy1/"
*/

#target photoshop

var fileName;
var fullPath;
var timelineData = "";
var offsetX = 0;
var offsetY = 0;
var delta = 0.07; 
var outputFolder =  "";
var psdFilesFolder = "";
var exportPath =  ""; 
var exportAttachments;
var anchorName = "anchor";
var attachmentName = "attachment_";

var windowResource = 
"dialog{ \
	text:'Sprite Animation Exporter v.0.9.0', bounds:[100,100,350,260],\
	dropdown:DropDownList{ bounds:[10,10,240,30],properties:{ items:['Multi Export', 'Single Export']}}\
	checkbox:Checkbox{ bounds:[10,40,100,60] , text:'Select folders' },\
	attachments:Checkbox{ bounds:[110,40,250,60] , text:'Export attachments' },\
	\
	anchorLabel:StaticText{bounds:[10,70,130,90] , text:'anchor name:' ,\
	properties:{scrolling:undefined,multiline:undefined}},\
	attachmentLabel:StaticText{bounds:[10,100,130,120] , text:'attachment name:' ,\
	properties:{scrolling:undefined,multiline:undefined}},\
	\
	anchorName:EditText{bounds:[140,70,240,90] , text:'anchor' ,\
	properties:{multiline:false,noecho:false,readonly:false}},\
	attachmentName:EditText{bounds:[140,100,240,120] , text:'attachment_' ,\
	properties:{multiline:false,noecho:false,readonly:false}},\
	\
	buttonStart:Button{ bounds:[10,130,110,150] , text:'Start'},\
	buttonCancel:Button{ bounds:[140,130,240,150] , text:'Cancel'},\
};";

var win = new Window(windowResource);

win.buttonStart.onClick = function()
{
	saveFilePath = Folder.userData.absoluteURI + "/animationExporter.txt";  	
	
	exportAttachments = win.attachments.value;
	
	if(win.anchorName.text != undefined && win.anchorName.text != '')
		anchorName = win.anchorName.text;
	if(win.attachmentName.text != undefined && win.attachmentName.text != '')
		attachmentName = win.attachmentName.text;
	
	if(win.dropdown.selection == 0)
		MultiExport(win.checkbox.value);
	else if(win.dropdown.selection == 1)
		SingleExport(win.checkbox.value);
	return win.close();
};

win.buttonCancel.onClick = function()
{
	return win.close();
};

function Main()
{		
	win.dropdown.selection = 0;
	win.show();
}

function CheckPaths()
{
	try
	{
		var saveFile = new File(saveFilePath);
		if(!saveFile.exists)
		{
			psdFilesFolder = "~/Desktop/";
			outFolder = "~/Desktop/";
			WritePaths();
		}
	}
	catch(exMessage)
	{
		alert(exMessage);
	}
}

function WritePaths()
{
	try
	{
		var saveFile = new File(saveFilePath);
		saveFile.open('w');
		saveFile.writeln(psdFilesFolder);
		saveFile.write(outputFolder);
		saveFile.close();
	}
	catch(exMessage)
	{
		alert(exMessage);
	}
}

function ReadPaths()
{
	try
	{
		var saveFile = new File(saveFilePath);
		saveFile.open('r');
		var inPath = saveFile.readln();
		var outPath = saveFile.readln();
		saveFile.close();
		psdFilesFolder = inPath;
		outputFolder = outPath;		
	}
	catch(exMessage)
	{
		alert(exMessage);
	}
}

function MultiExport(withSelectFolder)
{
	try
	{
		if(withSelectFolder)
		{
			psdFilesFolder = SelectDirectory("Выбрать папку с PSD");
			if(!psdFilesFolder)
				return;
			
			outputFolder = SelectDirectory("Выбрать выходную папку");
			if(!outputFolder)
				return;
			
			WritePaths();
		}
		else
		{
			CheckPaths();
			ReadPaths();
		}
		var start = new Date();
		Export(psdFilesFolder, outputFolder);
		var sec = (new Date() - start) / 1000;
		alert("Экспорт завершен! (" + sec + " sec)");
	}
	catch(exMessage)
	{
		alert(exMessage);
	}
}

function SingleExport(withSelectFolder)
{
	try
	{
		if(withSelectFolder)
		{
			outputFolder = SelectDirectory("Выбрать выходную папку");
			if(!outputFolder)
				return;
			WritePaths();
		}
		else
		{
			CheckPaths();
			ReadPaths();
		}
		var start = new Date();
		ExportAnimation(outputFolder);
		var sec = (new Date() - start) / 1000;
		alert("Экспорт завершен! (" + sec + " sec)");
	}
	catch(exMessage)
	{
		alert(exMessage);
	}
}

function Export(inFolder, outFolder)
{
	var files = ScanFolders(inFolder, ".psd");
	for(var i = 0; i < files.length; i++)
	{
		var refDoc = open(files[i]);
		ExportAnimation(outFolder);
		app.activeDocument.close(SaveOptions.DONOTSAVECHANGES);
	}
}

function ExportAnimation(outFolder)
{
	CheckDocument();
	timelineData = "";
	fileName = app.activeDocument.name.replace(".psd", "");
	exportPath = outFolder + "/";
	CreateSubDirectories();
	fullPath = exportPath + fileName;
	RunExport();
	WriteTimelineFile();
}

function ScanFolders(inFolder, mask)
{
	var folders = new Array();
	var allFiles = new Array();

	folders[0] = new Folder(inFolder);
	for(var i = 0; i < folders.length; i++)
	{
		var files = folders[i].getFiles();
		for(var j = 0; j < files.length; j++)
		{
			if(files[j] instanceof File)
            {
				if(mask == undefined)
					allFiles.push(files[j]);
				if(files[j].fullName.search(mask) > -1)
					allFiles.push(files[j]);
			}
			else
			{
				folders.push(files[j]);
				ScanFolders(files[i], mask);
			}
		}
	}
	return allFiles;
}

function WriteTimelineFile()
{
	var timelineFile = new File(fullPath  + ".txt");
	timelineFile.open('w');
	timelineData = timelineData.slice(0, -1);
	timelineFile.write(timelineData);
	timelineFile.close();
}

// if the filename is composite, we create subfolders for export
function CreateSubDirectories()
{
	var pathParts = fileName.split('_');
	if(pathParts.length > 1)
	{
		for(var i = 0; i < pathParts.length-1; i++)
			exportPath += pathParts[i] + "/";
	
		var tmpFolder = Folder(exportPath);
		if(!tmpFolder.exists) 
			tmpFolder.create();
		
		fileName = pathParts[pathParts.length-2] + "_" + pathParts[pathParts.length-1];
	}
}

function SelectDirectory(message)
{
	var folder = "~/Desktop/";
	return Folder(folder).selectDlg(message);
}

function CheckDocument()
{
	if(app.documents.length <= 0)
	{
		if(app.playbackDisplayDialogs != DialogModes.NO)
			alert("Нет открытых документов!");
		return 'cancel';
	}
}

function RunExport()
{
	var psd = app.activeDocument;	
	var history = psd.activeHistoryState;

	offsetX = 0;
	offsetY = 0;
	
	// set the units of measurement in pixels
	var savedRulerUnits = app.preferences.rulerUnits;
	var savedTypeUnits = app.preferences.typeUnits;
	app.preferences.rulerUnits = Units.PIXELS;
	app.preferences.typeUnits = TypeUnits.PIXELS; 
	app.activeDocument.activeLayer = psd.artLayers.add();
	
	var allLayers = [];
	allLayers = CollectAllLayers(app.activeDocument, allLayers, false);
	var anchorLayer = GetLayer(allLayers, anchorName);
	
	if(anchorLayer != null)
	{
		pos = GetPositionLayer(anchorLayer);
		if(pos)
		{
			offsetX = pos.x;
			offsetY = pos.y;
		}
	}
	
	if(anchorLayer != null)
		anchorLayer.remove();
		
	frameIndex = 0;
	
	while(SelectAnimationFrame(frameIndex + 1))
	{
		delta = GetDelay(frameIndex + 1);
		ExportPNGFrame(fullPath + "_" + frameIndex, fileName + "_" + frameIndex);
		frameIndex++;
	}
	
	psd.activeHistoryState = history;
}

// cut the layer in alpha and save it in .png
function ExportPNGFrame( outputFile, name )
{
	var psd = app.activeDocument;
		
	var history = psd.activeHistoryState;
	var data = "";
	if(exportAttachments)
	{
		var attachments = [];
		attachments = GetAttachments();	
		if(attachments.length > 0)
			data += " " + attachments.length;
		
		for(var i = 0; i < attachments.length; i++)
		{
			data += " " + attachments[i].name.replace(attachmentName, "");
			var p = GetPositionLayer(attachments[i]);
			data += " " + p.x + " " + p.y;
			attachments[i].remove();
		}
	}
		
	for (var i = 0; i < psd.layerSets.length; i++)
	{	
		if(psd.layerSets[i].visible && psd.layerSets[i].artLayers.length > 0)
			psd.layerSets[i].merge();
		else
			psd.layerSets[i].remove();
	}
	psd.mergeVisibleLayers();
	var pos;
	for (var i = 0; i < psd.artLayers.length; i++)
	{
		if(psd.artLayers[i].visible)
		{
			pos = GetPositionLayer(psd.artLayers[i]);
			break;
		}
	}	
	psd.trim(TrimType.TRANSPARENT);
	
	var pngFile = new File(outputFile);
	var pngSaveOptions = new PNGSaveOptions();
	app.activeDocument.saveAs(pngFile, pngSaveOptions, true, Extension.LOWERCASE);
	psd.activeHistoryState = history;
		
	timelineData += name + " " + pos.x + " " + pos.y + " " + delta; 
	if(exportAttachments)
		timelineData += data;
	timelineData += "\n";
}

function GetPositionLayer(activeLayer)
{
	var x = Number(activeLayer.bounds[0].value);
	var y = Number(activeLayer.bounds[3].value - app.activeDocument.height);
	y = Math.floor(y - offsetY);
	x = Math.floor(x - offsetX);
	return {'x': x, 'y': y}
}

function GetAttachments()
{
	var allLayers = [];
	var attachments = [];
	allLayers = CollectAllLayers(app.activeDocument, allLayers, true);
	for(var i = 0; i < allLayers.length; i++)
	{
		if(allLayers[i].name.indexOf(attachmentName) > -1)
			attachments.push(allLayers[i])
	}
	return attachments;
}

function SelectAnimationFrame( frameIndex )
{
	var idslct = charIDToTypeID( "slct" );
	var desc = new ActionDescriptor();
	var idnull = charIDToTypeID( "null" );
	var ref3 = new ActionReference();
	var idanimationFrameClass = stringIDToTypeID( "animationFrameClass" );
	ref3.putIndex( idanimationFrameClass, frameIndex );
	desc.putReference( idnull, ref3 );
	try
	{
		executeAction( idslct, desc, DialogModes.NO );
	}
	catch(exMessage)
	{
		return false;
	}
	return true;
}

function CollectAllLayers( layerHolder, allLayers, onlyVisibles )
{
	for( var i = 0; i < layerHolder.artLayers.length; ++i )
	{
		var layer_ = layerHolder.artLayers[i];
		if( layer_.visible || !onlyVisibles )
			allLayers.push( layer_ );
	}

	for( var j = 0; j < layerHolder.layerSets.length; ++j )
		CollectAllLayers( layerHolder.layerSets[j], allLayers, onlyVisibles );
	return allLayers;
}

function GetLayer(all, layerName)
{
	for(var i = 0; i < all.length; i++)
	{
		if(all[i].name == layerName)
			return all[i];
	}
	return null;
}

function GetDelay (theFrameIndex)   
{  
	var actionReference = new ActionReference();   
	actionReference.putProperty(charIDToTypeID('Prpr'), stringIDToTypeID('animationFrameDelay'));   
	actionReference.putIndex(stringIDToTypeID('animationFrameClass'), theFrameIndex);  
	var actionDescriptor = new ActionDescriptor();   
	actionDescriptor.putReference(charIDToTypeID('null'), actionReference);            
	var T = executeAction(charIDToTypeID('getd'), actionDescriptor, DialogModes.NO);  
	return T.getDouble(stringIDToTypeID('animationFrameDelay'))  
}  

Main();