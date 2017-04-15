Imports System.Security.Cryptography
Imports System.Text

Public Class SelectWallet
    Public ReadOnly Property MyTextValue As String
        Get
            Return Me.pass.EditValue

        End Get
    End Property
    Private Sub SelectWallet_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        pass.Text = ""
        LUESelectwallet.Properties.DataSource = callAPI.getPasswordTable
        LUESelectwallet.Properties.DisplayMember = "WalletName"
        LUESelectwallet.Properties.ValueMember = "WalletPass"
        LUESelectwallet.Properties.Columns(0).Width = 100
    End Sub
    
    Dim APICall As callAPI = New callAPI

    Private Sub SimpleButton1_Click(sender As Object, e As EventArgs) Handles SimpleButton1.Click
        'MsgBox(LUESelectwallet.EditValue.ToString)
        If TECode.Text = "" Then
            MsgBox("No Encryption Code Specified, Trying Default")
            pass.EditValue = APICall.AES_Decrypt(LUESelectwallet.EditValue, "burst").ToString
            SimpleButton1.DialogResult = System.Windows.Forms.DialogResult.OK
        Else
            pass.EditValue = APICall.AES_Decrypt(LUESelectwallet.EditValue, TECode.EditValue).ToString
            SimpleButton1.DialogResult = System.Windows.Forms.DialogResult.OK
        End If

    End Sub

End Class