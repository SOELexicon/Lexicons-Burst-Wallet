Imports DevExpress.LookAndFeel

Public Class Splash
    'Private lookAndFeel As UserLookAndFeel
    'Protected Overrides ReadOnly Property TargetLookAndFeel() As DevExpress.LookAndFeel.UserLookAndFeel
    '    Get
    '        If lookAndFeel Is Nothing Then
    '            lookAndFeel = New UserLookAndFeel(Me)
    '            lookAndFeel.UseDefaultLookAndFeel = False
    '            lookAndFeel.SkinName = "Darkroom"
    '        End If
    '        Return lookAndFeel
    '    End Get
    'End Property
    Sub New()

      
        InitializeComponent()


    End Sub

    Public Overrides Sub ProcessCommand(ByVal cmd As System.Enum, ByVal arg As Object)
        MyBase.ProcessCommand(cmd, arg)
    End Sub

    Public Enum SplashScreenCommand
        SomeCommandId
    End Enum

    Private Sub pictureEdit2_EditValueChanged(sender As Object, e As EventArgs) Handles pictureEdit2.EditValueChanged

    End Sub

    Private Sub Splash_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        With Me.DefaultLookAndFeel1
            .LookAndFeel.UseDefaultLookAndFeel = False
            .LookAndFeel.SkinName = "Darkroom"
        End With
        Me.DefaultLookAndFeel1.EnableBonusSkins = True
    End Sub
End Class
