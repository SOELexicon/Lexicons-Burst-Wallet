Imports System.Collections.Generic
Imports System.ComponentModel
Imports System.Data
Imports System.Drawing
Imports System.Drawing.Text
Imports System.Linq
Imports System.Text
Imports System.Windows.Forms
Imports System.Collections.Specialized
Imports System.Reflection
Imports System.Runtime.InteropServices
Imports System.Management.Instrumentation
Imports System.Management
Imports System.IO
Imports System.Threading
Imports Microsoft.VisualBasic.Devices
Imports System.Threading.Tasks

Public Class Plot_Files
    Dim highestStagger As Integer = 0
    Dim syncStagger As Integer = 0
    Dim safeMemStagger As Integer
    Dim HighestPlottedStartNonce As Integer = 0
    Dim Nonces_Per_Gigabyte As Double = 4096
    Dim plots As Double = 1
    Dim gigabyteInput As Double = 0
    Dim highestStaggerAllowed As Double = 20000
    Dim currentNonce As Double = 0
    Dim totalGigsPerPlot As Double
    Dim totalNonces As Double
    Dim Start_Nonce As Double
    Dim safememGB As ULong
    Dim safeMemMB As ULong
    Dim safeMemKB As ULong
    Dim memFull As ULong

    Dim synchronousPlotting As Boolean = False
    Dim plotterPlatform As Boolean = True
    Dim isAvx As Boolean = False
    Dim CanStart As Boolean = False
    Dim isNet As Boolean

    Dim genFilePath As String
    Dim plotterLoc As String
    Dim driveLetter As String
    Dim tempMem As String

    Dim currentDirectory As String

    Dim gpuPlotterDir As String = "\GPUPlotter"
    Dim cpuPlotterDir As String = "\XPlotter"
    Dim localIDFile As String = "\savedIds.conf"
    Dim setLocal As String = "@setlocal"
    Dim locChange As String = "@cd /d %~dp0"
    Dim optimize As String = "buffer"
    Dim gpuStartBat As String = "\AstroGpuStart.bat"
    Dim cpuStartBat As String = "\AstroCpuStart.bat"
    Dim gpuPlotEngineFile As String = "gpuPlotGenerator.exe"
    Dim cpuPlotEngineFile As String
    Dim BurstPath As String = "Burst/plots/"
    Dim pause As String = "pause"
    Dim atPause As String = "@pause"


    Dim drives As DriveInfo() = DriveInfo.GetDrives()

    Dim BeginningNonces As New List(Of Double)()
    Dim CommandsList As New List(Of String)()
    Dim UsedIds As New List(Of String)()
    Dim PlottedDir As New List(Of String)()
    Dim UsedPlotsNames As New List(Of String)()
    Dim UsedAccountNames As New List(Of String)()
    Dim UsedPlotsStartNonce As New List(Of String)()
    Dim UsedPlotsNonces As New List(Of String)()
    Dim CommandsArray As String()
    Dim NoncesArray As Double()
    Private Sub Plot_Files_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        LayoutControlItem9.Enabled = False
        Plots_Input.Properties.Minimum = 1

        LayoutControlItem1.Text = "Threads: 1"
        TrackBarControl1.Properties.Maximum = LogicalProcessorCount()
        currentDirectory = Directory.GetCurrentDirectory()
        drives = DriveInfo.GetDrives()
        PlottedDrives()
        Try

            If HasAvxSupport() = True Then
                isAvx = HasAvxSupport()
            Else
                isAvx = HasAvxSupport()
            End If
            plotterLoc = currentDirectory + cpuPlotterDir

            Bottom_Readout_Handler(0, 0)
        Catch ex As Exception
            MessageBox.Show(ex.ToString())
        End Try
        Num_ID_Input.Properties.Items.Clear()
        UsedIds.Clear()
        If File.Exists(currentDirectory + localIDFile) Then
            Using sr As New StreamReader(currentDirectory + "/" + localIDFile)
                Dim readLine As String = sr.ReadLine()
                While (readLine <> Nothing)
                    UsedIds.Add(readLine)
                    readLine = sr.ReadLine()
                End While
                For i As Integer = 0 To UsedIds.Count - 1
                    Num_ID_Input.Properties.Items.Add(UsedIds(i))
                Next
            End Using
        End If

        Array.Clear(drives, 0, drives.Length)
        WriteToDriveLetter.Properties.Items.Clear()
        drives = DriveInfo.GetDrives()
        For i As Integer = 0 To drives.Length - 1
            Try

                If drives(i).DriveType.ToString() = "Removeable" OrElse drives(i).DriveType.ToString() = "Fixed" OrElse drives(i).DriveType.ToString() <> "CDRom" AndAlso drives(i).IsReady = True Then
                    Dim gbTwoPlaces As Double = Math.Round((((CDbl(drives(i).AvailableFreeSpace) / 1024) / 1024) / 1024), 2)
                    WriteToDriveLetter.Properties.Items.Add(drives(i).ToString() + " " + gbTwoPlaces.ToString() + " GB" + " [" + drives(i).VolumeLabel + "]")
                Else
                    If drives(i).DriveType.ToString() <> "CDRom" AndAlso drives(i).IsReady = True Then
                        WriteToDriveLetter.Properties.Items.Add(drives(i).ToString() + " [" + drives(i).VolumeLabel + "]")
                    End If
                End If
            Catch ex As Exception
                MessageBox.Show("Unmounted drive letters" & vbLf & "Do you have card readers or" & vbLf & " Dead drives attached?        " + drives(i).ToString() + "         " + ex.ToString())
            End Try
        Next
        DevExpress.XtraSplashScreen.SplashScreenManager.CloseForm()

    End Sub

    <System.Runtime.InteropServices.DllImport("gdi32.dll")> _
    Private Shared Function AddFontMemResourceEx(pbFont As IntPtr, cbFont As UInteger, pdv As IntPtr, <System.Runtime.InteropServices.In> ByRef pcFonts As UInteger) As IntPtr
    End Function

    <System.Runtime.InteropServices.DllImport("kernel32.dll")> _
    Private Shared Function GetEnabledXStateFeatures() As Long
    End Function

    Private fonts As New PrivateFontCollection()


    Private myFont As Font

    Private Sub WriteToDriveLetter_Click(sender As Object, e As EventArgs) Handles WriteToDriveLetter.Click
 
    End Sub

    Private Sub WriteToDriveLetter_SelectedIndexChanged(sender As Object, e As EventArgs) Handles WriteToDriveLetter.SelectedIndexChanged
        Dim WriteToDrivePath As String = WriteToDriveLetter.Text
        driveLetter = WriteToDriveLetter.SelectedItem.ToString()
        Dim index As Integer = driveLetter.IndexOf("\")
        If index > 0 Then
            driveLetter = driveLetter.Substring(0, index)
        End If


        Dim comboValue As Integer = CInt(WriteToDriveLetter.SelectedIndex)
        If drives(comboValue).DriveType.ToString() = "Network" Then
            isNet = True
        Else
            isNet = False
        End If


        'label7.Text = driveLetter;
    End Sub

    Private Sub PlotTypeBool_SelectedIndexChanged(sender As Object, e As EventArgs) Handles PlotTypeBool.SelectedIndexChanged
        If PlotTypeBool.SelectedItem.ToString() = "Buffer(Faster)" Then
            optimize = "buffer"
        Else
            optimize = "direct"
        End If
    End Sub

    Private Sub HardwarePlatform_SelectedIndexChanged(sender As Object, e As EventArgs) Handles HardwarePlatform.SelectedIndexChanged
        Try
            If HardwarePlatform.SelectedItem.ToString() = "Cpu" Then
                plotterLoc = currentDirectory + cpuPlotterDir
                'label7.Text = plotterLoc;
                plotterPlatform = True
                LayoutControlItem9.Enabled = False

            Else
                plotterLoc = currentDirectory + gpuPlotterDir
                'label7.Text = plotterLoc;
                plotterPlatform = False
                LayoutControlItem9.Enabled = True
            
            End If
        Catch ex As Exception
            MessageBox.Show("Caught exception in HardwarePlatormChanged" + ex.ToString())
        End Try
    End Sub

    Private Sub Commit_Click(sender As Object, e As EventArgs)

    End Sub

    Private Sub StartPlotterButton_Click(sender As Object, e As EventArgs) Handles StartPlotterButton.Click
        'System.Diagnostics.Process.Start(genFilePath);
        Try
            If GB_Input.Text <> "" AndAlso WriteToDriveLetter.Text <> "" AndAlso Num_ID_Input.Text <> "" Then
                CanStart = True
            Else
                CanStart = False
                MessageBox.Show("Please check your options then re-commit!")
            End If

            If CanStart Then
                StartPlotterButton.Enabled = True


                If UsedIds.Contains(Num_ID_Input.Text) = False Then
                    UsedIds.Add(Num_ID_Input.Text)
                End If

                If File.Exists(currentDirectory + "/" + localIDFile) Then
                    File.Delete(currentDirectory + "/" + localIDFile)
                End If
                File.WriteAllLines(currentDirectory + "/" + localIDFile, UsedIds)


                FindStartNonces()
                CreateCommands()
                WriteFile()
            End If
            If isNet = False Then
                Dim proc As New System.Diagnostics.ProcessStartInfo() With { _
                     .UseShellExecute = True, _
                     .FileName = genFilePath, _
                     .WorkingDirectory = currentDirectory, _
                     .Verb = "runas" _
                }
                System.Diagnostics.Process.Start(proc)
                Thread.Sleep(100)
                Me.Close()
            Else
                System.Diagnostics.Process.Start(genFilePath)
                Thread.Sleep(100)
                Me.Close()
            End If
        Catch ex As Exception
            MessageBox.Show("found fault in process start " + ex.ToString())
        End Try

    End Sub

    Private Sub GB_Input_TextChanged(sender As Object, e As EventArgs) Handles GB_Input.EditValueChanged
        RamUsagePuller()
        TotalNoncesMath()
    End Sub

    Private Sub GB_Input_KeyPress(sender As Object, e As KeyPressEventArgs)
        If Not Char.IsControl(e.KeyChar) AndAlso Not Char.IsDigit(e.KeyChar) AndAlso (e.KeyChar <> "."c) Then
            e.Handled = True
        End If
        If System.Text.RegularExpressions.Regex.IsMatch(GB_Input.Text, "\.\d\d") AndAlso e.KeyChar <> "8" Then
            '&& System.Text.RegularExpressions.Regex.IsMatch(GB_Input.Text, "^ [0-9]")
            e.Handled = True
        End If
    End Sub

    Private Sub GB_Input_KeyUp(sender As Object, e As KeyEventArgs)
        If GB_Input.Text = "" Then
            gigabyteInput = 0
            totalNonces = 0
            TotalNoncesMath()
        End If
    End Sub

    Private Sub Num_ID_Input_TextChanged(sender As Object, e As EventArgs)
        'if (System.Text.RegularExpressions.Regex.IsMatch(Num_ID_Input.Text, "^ [0-9]")) ;
        '{
        'Sends to nil or something like that...  
        '}
    End Sub

    Private Sub Num_ID_Input_KeyPress(sender As Object, e As KeyPressEventArgs)
        If Not Char.IsControl(e.KeyChar) AndAlso Not Char.IsDigit(e.KeyChar) Then
            e.Handled = True
        End If
    End Sub

    Private Sub Plots_Input_Scroll(sender As Object, e As EventArgs) Handles Plots_Input.EditValueChanged
        TotalNoncesMath()
        RamUsagePuller()

    End Sub

    Public Sub TotalNoncesMath()
        Try
            If Double.TryParse(GB_Input.Text, gigabyteInput) Then
                plots = Plots_Input.Value
                totalNonces = Math.Floor((gigabyteInput * Nonces_Per_Gigabyte) / plots)
            End If
            GigsPerPlotMath()
        Catch ex As Exception
            MessageBox.Show("Caught exception in NonceMath" + ex.ToString())
        End Try
    End Sub

    Public Sub GigsPerPlotMath()
        Try
            Plots_Readout.Text = "Plots " + plots.ToString()
            totalGigsPerPlot = Math.Round(gigabyteInput / plots, 2)
            FindStagger()
            Bottom_Readout_Handler(totalGigsPerPlot, totalNonces)
        Catch ex As Exception
            MessageBox.Show("Caught exception in GbMath" + ex.ToString())
        End Try
    End Sub
    Private Sub Bottom_Readout_Handler(Gb_Plot As Double, Nonces_Plot As Double)
        RamUsagePuller()
        Bottom_Readout.Text = "Gb/Plot= " + Gb_Plot.ToString() + ", Nonces/Plot= " + Nonces_Plot.ToString() + " Stagger/Plot= " + highestStagger.ToString()
        ReadoutHolder.Text = " Ram= (" + (highestStagger / 4).ToString() + "/" + safeMemMB.ToString() + ")/" + memFull.ToString() + " Threads= " + LogicalProcessorCount().ToString() + "/" + Environment.ProcessorCount.ToString() + ", StartNonce= " + currentNonce.ToString()
    End Sub

    Private Sub RamUsagePuller()
        Try
            Dim CI As New ComputerInfo()
            Dim mem As ULong = ULong.Parse(CI.AvailablePhysicalMemory.ToString())
            memFull = mem / (1024 * 1024)
            Dim safeMem As ULong = mem - (mem / 4)

            safememGB = (safeMem / ((1024 * 1024) * 1024))
            safeMemMB = (safeMem / (1024 * 1024))
            safeMemKB = (safeMem / 1024)
            'testlab.Text = (mem / (1024 * 1024) + "MB");


            safeMemStagger = CInt(safeMemMB) * 4
            'MessageBox.Show("safemem " + safeMemMB + " safememstagger " + safeMemStagger);

            highestStaggerAllowed = safeMemKB / 256
        Catch ex As Exception
            MessageBox.Show("Caught exception in RamPuller" + ex.ToString())
        End Try
    End Sub

    Public Shared Function HasAvxSupport() As Boolean
        Try
            Return (GetEnabledXStateFeatures() And 4) <> 0
        Catch
            Return False
        End Try
    End Function

    Public Shared Function LogicalProcessorCount() As Integer

        If Environment.ProcessorCount = 1 Then
            Return Environment.ProcessorCount
        Else
            Return Environment.ProcessorCount - 1
        End If
    End Function

    Private Sub PlottedDrives()
        Try
            For i As Integer = 0 To drives.Length - 1
                If drives(i).DriveType.ToString() <> "CDRom" OrElse drives(i).DriveType.ToString() <> "Unknown" Then
                    If Directory.Exists(drives(i).ToString() + BurstPath) Then
                        PlottedDir.Add(drives(i).ToString() + BurstPath)
                    End If
                End If
            Next
            'File.WriteAllLines(currentDirectory + "/log.txt", PlottedDir);
            For i As Integer = 0 To PlottedDir.Count - 1
                Dim dirPlots As String() = Directory.GetFiles(PlottedDir(i))
                UsedPlotsNames.AddRange(dirPlots)
                UsedAccountNames.AddRange(dirPlots)
            Next
            File.WriteAllLines(currentDirectory + "/logPlots.txt", UsedPlotsNames)
            For i As Integer = 0 To UsedPlotsNames.Count - 1
                Dim workingString As String = UsedPlotsNames(i)
                UsedAccountNames(i) = workingString.Substring(workingString.IndexOf("plots/") + 6)
                UsedAccountNames(i) = UsedAccountNames(i).Substring(0, UsedAccountNames(i).IndexOf("_"))
                workingString = workingString.Substring(workingString.IndexOf("_") + 1)
                workingString = workingString.Substring(0, workingString.LastIndexOf("_"))
                UsedPlotsNames(i) = workingString
            Next
            'File.WriteAllLines(currentDirectory + "/logPlotsCore.txt", UsedPlotsNames);
            For i As Integer = 0 To UsedPlotsNames.Count - 1
                Dim stringCore As String = UsedPlotsNames(i)
                Dim workingString As String() = stringCore.Split("_"c)
                UsedPlotsStartNonce.Add(workingString(0))
                UsedPlotsNonces.Add(workingString(1))
            Next
            'File.WriteAllLines(currentDirectory + "/logPlotsStartNonces.txt", UsedPlotsStartNonce);
            'File.WriteAllLines(currentDirectory + "/logPlotsNonces.txt", UsedPlotsNonces);
            For i As Integer = 0 To UsedPlotsStartNonce.Count - 1
                If Integer.Parse(UsedPlotsStartNonce(i)) > HighestPlottedStartNonce Then
                    HighestPlottedStartNonce = Integer.Parse(UsedPlotsStartNonce(i))
                End If

                currentNonce = (HighestPlottedStartNonce + Integer.Parse(UsedPlotsNonces(i)) + 1)

                Start_Nonce = currentNonce
                'MessageBox.Show(currentNonce.ToString());
            Next
        Catch ex As Exception
            MessageBox.Show("Couldnt gather start nonce data! " + ex.ToString())
        End Try
    End Sub

    Dim threadsxplotter As Integer

    Private Sub FindStagger()
        Try
            Dim FindStagger As Task = Task.Run(Sub()
                                                   For factor As Integer = 1 To totalNonces
                                                       'test from 1 to the square root, or the int below it, inclusive.
                                                       If totalNonces Mod factor = 0 Then
                                                           If factor <> totalNonces AndAlso factor <= highestStaggerAllowed Then
                                                               highestStagger = factor
                                                               If highestStagger < 1024 Then
                                                                   highestStagger = CInt(highestStaggerAllowed) + (CInt(highestStaggerAllowed) \ CInt(9.99))
                                                               End If
                                                           End If
                                                       End If
                                                   Next

                                               End Sub)
        Catch ex As Exception
            MessageBox.Show("Caught exception in StaggerCalc" + ex.ToString())
        End Try
        Bottom_Readout_Handler(totalGigsPerPlot, totalNonces)
    End Sub

    Private Sub FindStartNonces()
        Try
            BeginningNonces.Clear()
            currentNonce = Start_Nonce
            For nonceCalc As Integer = 1 To Plots_Input.Value
                BeginningNonces.Add(currentNonce)
                currentNonce = (currentNonce + totalNonces) + 1
            Next
            NoncesArray = BeginningNonces.ToArray()
        Catch ex As Exception
            MessageBox.Show("Caught exception in StartNonceCalc" + ex.ToString())
        End Try
    End Sub

    Private Sub CreateCommands()
        If plotterPlatform = False Then
            Try
                If plotterLoc.StartsWith("C") Then
                    CommandsList.Add("cd " + plotterLoc)
                Else
                    CommandsList.Add(plotterLoc.Substring(0, 2))
                    CommandsList.Add("cd " + plotterLoc)
                End If


                If synchronousPlotting = False Then
                    For genCom As Integer = 0 To Plots_Input.Value - 1
                        Dim Command As String = gpuPlotEngineFile + " generate " + optimize + " " + driveLetter + "//" + BurstPath + Num_ID_Input.Text + "_" + NoncesArray(genCom).ToString() + "_" + totalNonces.ToString() + "_" + highestStagger.ToString()
                        CommandsList.Add(Command)
                    Next
                    CommandsList.Add(pause)
                    CommandsArray = CommandsList.ToArray()
                    CommandsList.Clear()
                Else
                    syncStagger = highestStagger / Plots_Input.Value
                    Dim Command As String = gpuPlotEngineFile + " generate " + optimize + " " + driveLetter + "//" + BurstPath + Num_ID_Input.Text + "_" + NoncesArray(0).ToString() + "_" + totalNonces.ToString() + "_" + syncStagger.ToString()
                    CommandsList.Add(Command)
                    For genCom As Integer = 1 To Plots_Input.Value - 1
                        Command = " " + driveLetter + "//" + BurstPath + Num_ID_Input.Text + "_" + NoncesArray(genCom).ToString() + "_" + totalNonces.ToString() + "_" + syncStagger.ToString()
                        CommandsList.Add(Command)
                    Next
                    CommandsList.Add(pause)
                    CommandsArray = CommandsList.ToArray()
                    CommandsList.Clear()
                End If
            Catch ex As Exception
                MessageBox.Show("Caught exception in CommandCreator" + ex.ToString())
            End Try
        Else
            Try
                If isAvx = True Then
                    cpuPlotEngineFile = "XPlotter_avx.exe"
                Else
                    cpuPlotEngineFile = "XPlotter_sse.exe"
                End If
                CommandsList.Clear()
                CommandsList.Add(setLocal)
                CommandsList.Add(locChange)
                If safeMemMB < 1024 Then
                    tempMem = safeMemMB.ToString() + "M"
                Else
                    tempMem = safememGB.ToString() + "G"
                End If


                For genCom As Integer = 0 To Plots_Input.Value - 1
                    'string Command = gpuPlotEngineFile + " generate " + optimize + " " + driveLetter + "//" + BurstPath + Num_ID_Input.Text.ToString() + "_" + NoncesArray[genCom].ToString() + "_" + totalNonces.ToString() + "_" + highestStagger.ToString();
                    Dim Command As String = cpuPlotEngineFile + " -id " + Num_ID_Input.Text + " -sn " + NoncesArray(genCom).ToString() + " -n " + totalNonces.ToString() + " -t " + threadsxplotter.ToString() + " -path " + driveLetter + "//" + BurstPath + " -mem " + tempMem
                    CommandsList.Add(Command)
                Next
                CommandsList.Add(atPause)
                CommandsArray = CommandsList.ToArray()
                CommandsList.Clear()
            Catch ex As Exception
                MessageBox.Show("Caught exception in CpuCommandWriter " + gpuStartBat + " " + ex.ToString())
            End Try
        End If
        If Directory.Exists(driveLetter + "/" + BurstPath) = False Then
            Directory.CreateDirectory(driveLetter + "/" + BurstPath)
        End If
    End Sub

    Private Sub WriteFile()
        If plotterPlatform = False Then
            Try
                genFilePath = plotterLoc + gpuStartBat
                If File.Exists(genFilePath) Then
                    File.Delete(genFilePath)
                End If
                File.WriteAllLines(genFilePath, CommandsArray)
            Catch ex As Exception
                MessageBox.Show("Caught exception in gpufilewriter " + gpuStartBat + " " + ex.ToString())
            End Try
        Else
            Try
                genFilePath = plotterLoc + cpuStartBat
                If File.Exists(genFilePath) Then
                    File.Delete(genFilePath)
                End If
                File.WriteAllLines(genFilePath, CommandsArray)
            Catch ex As Exception
                MessageBox.Show("Caught exception in cpufilewriter " + cpuStartBat + " " + ex.ToString())
            End Try
        End If

    End Sub

    Private Sub TrackBarControl1_EditValueChanged(sender As Object, e As EventArgs) Handles TrackBarControl1.EditValueChanged
        threadsxplotter = TrackBarControl1.EditValue
        LayoutControlItem1.Text = "Threads: " + threadsxplotter.ToString()
    End Sub


    Private Sub btnResume_Click(sender As Object, e As EventArgs) Handles btnResume.Click
        Try
            Dim proc As New System.Diagnostics.ProcessStartInfo() With { _
                      .UseShellExecute = True, _
                      .FileName = "AstroCpuStart.bat", _
                      .WorkingDirectory = currentDirectory & cpuPlotterDir, _
                      .Verb = "runas" _
                 }
            System.Diagnostics.Process.Start(proc)
        Catch ex As Exception

        Finally
            Thread.Sleep(100)
            Me.Close()
        End Try

      
    End Sub
End Class