<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class Mine
    Inherits DevExpress.XtraEditors.XtraForm

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        If disposing AndAlso components IsNot Nothing Then
            components.Dispose()
        End If
        MyBase.Dispose(disposing)
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.RangeTrackBarControl1 = New DevExpress.XtraEditors.RangeTrackBarControl()
        CType(Me.RangeTrackBarControl1, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.RangeTrackBarControl1.Properties, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'RangeTrackBarControl1
        '
        Me.RangeTrackBarControl1.EditValue = New DevExpress.XtraEditors.Repository.TrackBarRange(0, 0)
        Me.RangeTrackBarControl1.Location = New System.Drawing.Point(296, 201)
        Me.RangeTrackBarControl1.Name = "RangeTrackBarControl1"
        Me.RangeTrackBarControl1.Properties.LabelAppearance.Options.UseTextOptions = True
        Me.RangeTrackBarControl1.Properties.LabelAppearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center
        Me.RangeTrackBarControl1.Size = New System.Drawing.Size(358, 56)
        Me.RangeTrackBarControl1.TabIndex = 0
        '
        'Mine
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(7.0!, 16.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(1143, 444)
        Me.Controls.Add(Me.RangeTrackBarControl1)
        Me.Name = "Mine"
        Me.Text = "Mine"
        CType(Me.RangeTrackBarControl1.Properties, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.RangeTrackBarControl1, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents RangeTrackBarControl1 As DevExpress.XtraEditors.RangeTrackBarControl
End Class
