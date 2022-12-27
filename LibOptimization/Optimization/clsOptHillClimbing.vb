Imports LibOptimization.Util
Imports LibOptimization.MathTool

Namespace Optimization
    ''' <summary>
    ''' Hill-Climbing algorithm(山登り法)
    ''' </summary>
    ''' <remarks>
    ''' Features:
    '''  -Randomized algorithm for optimization.
    ''' 
    ''' Reffrence:
    ''' https://en.wikipedia.org/wiki/Hill_climbing
    ''' 
    ''' Implment:
    ''' N.Tomi(tomi.nori+github at gmail.com)
    ''' </remarks>
    <Serializable>
    Public Class clsOptHillClimbing : Inherits absOptimization
#Region "Member"
        '----------------------------------------------------------------
        'Common parameters
        '----------------------------------------------------------------
        ''' <summary>point</summary>
        Private _populations As New List(Of clsPoint)

        ''' <summary>Max iteration count</summary>
        Public Overrides Property Iteration As Integer = 10000

        ''' <summary>Upper bound(limit solution space)</summary>
        Public Property UpperBounds As Double() = Nothing

        ''' <summary>Lower bound(limit solution space)</summary>
        Public Property LowerBounds As Double() = Nothing

        '----------------------------------------------------------------
        'Hill-Climbing parameters
        '----------------------------------------------------------------
        ''' <summary>range of neighbor search</summary>
        Public Property NeighborRange As Double = 0.1

        ''' <summary>range of neighbor search</summary>
        Public Property NeighborSize As Integer = 50
#End Region

#Region "Constructor"
        ''' <summary>
        ''' Constructor
        ''' </summary>
        ''' <param name="ai_func">Objective Function</param>
        ''' <remarks>
        ''' </remarks>
        Public Sub New(ByVal ai_func As absObjectiveFunction)
            Me.m_func = ai_func
        End Sub
#End Region

#Region "Public"
        ''' <summary>
        ''' Init
        ''' </summary>
        ''' <remarks></remarks>
        Public Overrides Sub Init()
            Try
                'init meber varibles
                Me.m_iteration = 0
                Me._populations.Clear()
                Me.m_error.Clear()

                'check initialposition
                If MyBase.InitialPosition IsNot Nothing Then
                    If MyBase.InitialPosition.Length = MyBase.m_func.NumberOfVariable Then
                        'nothing
                    Else
                        Throw New ArgumentException("The number of variavles in InitialPosition and objective function are different.")
                    End If
                End If

                'init initial position
                If InitialPosition IsNot Nothing Then
                    Me._populations.Add(New clsPoint(Me.m_func, InitialPosition))
                Else
                    Dim array = clsUtil.GenRandomPositionArray(Me.m_func, InitialPosition, Me.InitialValueRangeLower, Me.InitialValueRangeUpper)
                    Me._populations.Add(New clsPoint(Me.m_func, array))
                End If
            Catch ex As Exception
                Me.m_error.SetError(True, clsError.ErrorType.ERR_INIT)
            End Try
        End Sub

        Public Count As Integer = 0

        ''' <summary>
        ''' Do Iteration
        ''' </summary>
        ''' <param name="ai_iteration">Iteration count. When you set zero, use the default value.</param>
        ''' <returns>True:Stopping Criterion. False:Do not Stopping Criterion</returns>
        ''' <remarks></remarks>
        Public Overrides Function DoIteration(Optional ByVal ai_iteration As Integer = 0) As Boolean
            'Check Last Error
            If Me.IsRecentError() = True Then
                Return True
            End If

            'Do Iterate
            If Me.Iteration <= m_iteration Then
                Return True
            Else
                ai_iteration = If(ai_iteration = 0, Iteration - m_iteration - 1, Math.Min(ai_iteration, Iteration - m_iteration) - 1)
            End If
            For iterate As Integer = 0 To ai_iteration
                'Counting generation
                m_iteration += 1

                'neighbor function
                Dim nextPoint = Neighbor(_populations(0))

                'limit solution space
                clsUtil.LimitSolutionSpace(nextPoint, LowerBounds, UpperBounds)

                'evaluate
                If _populations(0).Eval > nextPoint.Eval Then
                    _populations(0) = nextPoint
                End If
            Next

            Return False
        End Function

        ''' <summary>
        ''' Best result
        ''' </summary>
        ''' <returns>Best point class</returns>
        ''' <remarks></remarks>
        Public Overrides ReadOnly Property Result As clsPoint
            Get
                Return _populations(0)
            End Get
        End Property

        ''' <summary>
        ''' All Result
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>
        ''' for Debug
        ''' </remarks>
        Public Overrides ReadOnly Property Results As List(Of clsPoint)
            Get
                Return _populations
            End Get
        End Property
#End Region

#Region "Private"
        ''' <summary>
        ''' Neighbor function for local search
        ''' </summary>
        ''' <param name="base"></param>
        ''' <returns></returns>
        Private Function Neighbor(ByVal base As clsPoint) As clsPoint
            Dim ret As New List(Of clsPoint)
            For k As Integer = 0 To Me.NeighborSize - 1
                Dim temp As New clsPoint(base)
                For i As Integer = 0 To temp.Count - 1
                    Dim tempNeighbor = Math.Abs(2.0 * NeighborRange) * MyBase.Random.NextDouble() - NeighborRange
                    temp(i) += tempNeighbor
                Next
                temp.ReEvaluate()
                ret.Add(temp)
            Next
            ret.Sort()

            Return ret(0)
        End Function
#End Region
    End Class
End Namespace
