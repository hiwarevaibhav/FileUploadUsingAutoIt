WinActivate("Open")
WinWaitActive("Open")
ControlSend("Open", "", "Edit1", $CmdLine[1])
ControlClick("Open", "&Open", "Button1")
