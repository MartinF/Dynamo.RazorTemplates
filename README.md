# Generate Templates Server-side
Dynamo.RazorTemplates is a server-side template compiler, that compiles your server-side templates into client-side code (JavaScript).

A template is defined by the razor @helper syntax.

```HTML
@helper MyHelper_Tmpl(int count) {
	<h1>Hello world! - @count</h1>
}
```

The template will be compiled into the following client-side template (*formatted for nice display)

```JAVASCRIPT
function MyHelper_Tmpl(count) { 
	var t="";
	t+="<h1>Hello world! - ";
	t+=count;
	t+="</h1>";
	return t; 
}
```

## Benefits
- No duplicate code to keep in sync - update server-side and it is reflected client-side.
- Not yet another JS library to include (saves a request, KB across the wire and browser load time)
- Pre-compiled - Fast and Saves processing time

## Help
If you find this project useful and want to help with improving it, you are more than welcome.
Please report any errors you find.

## Notes
- Check the tests for more advanced examples.
- This a proto-type project. 
- Check out my Jiss project for a way to easily compile templates on Visual Studio triggers (eg. save).
