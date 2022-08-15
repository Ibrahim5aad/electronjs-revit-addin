
module.exports = {
    arrayBufferToBase64: function ( buffer ) {
        var binary = '';
        var bytes = new Uint8Array( buffer );
        var len = bytes.byteLength;
        for (var i = 0; i < len; i++) {
            binary += String.fromCharCode( bytes[ i ] );
        }
        let buff = Buffer.from(binary); 
        return buff.toString('base64');
    },

     arrayBufferToString: function (buffer){

        var binary = '';
        var bytes = new Uint8Array( buffer );
        var len = bytes.byteLength;
        for (var i = 0; i < len; i++) {
            binary += String.fromCharCode( bytes[ i ] );
        }
        let buff = Buffer.from(binary);
        return buff.toString();
    
    }

 }


