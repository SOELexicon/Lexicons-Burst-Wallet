Public Class Burst
    Sub New()
        DevExpress.LookAndFeel.UserLookAndFeel.Default.SetSkinStyle("Darkroom")
        InitializeComponent()
    End Sub

    Public Overrides Sub ProcessCommand(ByVal cmd As System.Enum, ByVal arg As Object)
        MyBase.ProcessCommand(cmd, arg)
    End Sub

    Public Enum SplashScreenCommand
        SomeCommandId
    End Enum

    Private Sub pictureEdit2_EditValueChanged(sender As Object, e As EventArgs)

    End Sub
End Class
