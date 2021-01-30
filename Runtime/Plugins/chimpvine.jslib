var LibraryHttpCookie = {

getHttpCookies: function()
{
	var cookies = document.cookie;
	var length = lengthBytesUTF8(cookies) + 1;
	var buffer = _malloc(length);
	stringToUTF8(cookies, buffer, length);
	return buffer;
},

getHttpCookie: function(nameArg)
{
	var name = Pointer_stringify(nameArg);
	var cookie = document.cookie;
	var search = name + "=";
	var setStr = "";
	var offset = 0;
	var end = 0;
	if (cookie.length > 0)
	{
		offset = cookie.indexOf(search);
		if (offset != -1)
		{
			offset += search.length;
			end = cookie.indexOf(";", offset);
			if (end == -1)
			{
				end = cookie.length;
			}
			setStr = decodeURI(cookie.substring(offset, end));
		}
	}

	var length = lengthBytesUTF8(setStr) + 1;
	var buffer = _malloc(length);
	stringToUTF8(setStr, buffer, length);
	return buffer;
}
};

var LibraryUtilities = {
	
	isMobileBrowser: function () {
    return (/iPhone|iPad|iPod|Android/i.test(navigator.userAgent));
  },

  showAlert : function(str){
    window.alert(Pointer_stringify(str));
  }
};

var LibraryURL = {
	
	getURLFromPage : function(){
		var returnStr = window.top.location.href;
		var bufferSize = lengthBytesUTF8(returnStr) + 1;
		var buffer = _malloc(bufferSize);
		stringToUTF8(returnStr, buffer, bufferSize);
		return buffer;
	}
};

mergeInto(LibraryManager.library, LibraryHttpCookie);
mergeInto(LibraryManager.library, LibraryUtilities);
mergeInto(LibraryManager.library, LibraryURL);