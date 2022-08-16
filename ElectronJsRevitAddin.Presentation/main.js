var net = require('net'); 
const { app, BrowserWindow, ipcMain } = require('electron')
const Schema = require("./proto/Request_pb")
const path = require("path");  
const Utils = require('./utils/helper-functions.js');
 
var pipePath = "\\\\.\\pipe\\Pipe.Server";

let win;


var client = net.connect(pipePath, connectionListener = function() 
	{
		client.on('data', (data) => { 
            var strData = Utils.arrayBufferToString(data);
            win.webContents.send("upstream", strData.trim());
	});
});
  
  
const createWindow = () => {
  win = new BrowserWindow({
    width: 600,
    height: 600,
    minHeight: 600,
    minWidth: 600,
    autoHideMenuBar: true,
    alwaysOnTop: true,
    title: "Electron.js Revit UI",
    webPreferences: { 
        nodeIntegration: false, // is default value after Electron v5
        contextIsolation: true, // protect against prototype pollution
        enableRemoteModule: false, // turn off remote
        preload: path.join(__dirname, "utils/preload.js")
    }
  }) 
  win.loadFile("index.html")
}

app.whenReady().then(() => {
  createWindow()

  win.on('close', function() {  
    var request = new Schema.Request();
    request.setEndpoint('Window/Close');
    request.setPayload(JSON.stringify( { "isclose" : true }));
    var data = Utils.arrayBufferToBase64(request.serializeBinary());
    client.write(data);
  });

  // var handle = win.getNativeWindowHandle();
  // var requestHandle = new Schema.Request();
  // requestHandle.setEndpoint("Main/SetWindowOwner");
  // requestHandle.setPayload(JSON.stringify({ "WindowHandle" : handle}));
  // var data = Utils.arrayBufferToBase64(requestHandle.serializeBinary());
  // client.write(data);

  app.on('activate', () => { 
    if (BrowserWindow.getAllWindows().length === 0) 
      createWindow()
  }); 
})

ipcMain.on("downstream", (event, args) => { 
        var requestJson = JSON.parse(args);
        var request = new Schema.Request();
        request.setEndpoint(requestJson.endpoint);
        request.setPayload(requestJson.payload);
        var data = Utils.arrayBufferToBase64(request.serializeBinary());
        client.write(data);
      
  }); 

app.on('window-all-closed', () => {
    if (process.platform !== 'darwin') app.quit()
  });

 
