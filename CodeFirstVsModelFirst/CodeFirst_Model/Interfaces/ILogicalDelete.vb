Imports System.ComponentModel

Namespace Interfaces
    Public Interface ILogicalDelete

        ''' <summary>
        ''' Gibt zurück ob die Entität als Gelöscht markiert ist oder setzt den Wert!!
        ''' </summary>
        <DefaultValue(False)>
        Property DeletedFlag As Boolean

        Property DeletedTimestamp As DateTime?
    End Interface
End Namespace