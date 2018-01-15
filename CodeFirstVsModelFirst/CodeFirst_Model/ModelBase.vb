Imports System.ComponentModel.DataAnnotations

Public MustInherit Class ModelBase

    Public Overridable Property CreatedBy As String = Environment.UserName
    Public Overridable Property CreatedOn As String = Environment.MachineName
    Public Overridable Property LastUpdateBy As String = Environment.UserName
    Public Overridable Property LastUpdateOn As String = Environment.MachineName
    <Timestamp>
    Public Overridable Property RowVersion As Byte()



End Class
