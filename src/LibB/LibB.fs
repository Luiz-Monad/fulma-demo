module LibB

open Browser.Dom

let showInConsole (msg : string) = console.log(msg)

showInConsole "debug lazy"
