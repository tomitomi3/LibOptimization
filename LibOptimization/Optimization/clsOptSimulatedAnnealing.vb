Imports LibOptimization.Util
Imports LibOptimization.MathUtil

Namespace Optimization

    ''' <summary>
    ''' abstract neighbor function for Simulated Annealing
    ''' </summary>
    Public MustInherit Class absNeighbor
        Public MustOverride Function Neighbor(ByVal base As clsPoint) As clsPoint
    End Class

    ''' <summary>
    ''' Local random search (default)
    ''' </summary>
    Public Class LocalRandomSearch : Inherits absNeighbor
        ''' <summary>local search range</summary>
        Public Property NeighborRange As Double = 0.01

        ''' <summary>random class</summary>
        Public Property Random As System.Random = New Util.clsRandomXorshift()

        ''' <summary>
        ''' search function
        ''' </summary>
        ''' <param name="base"></param>
        ''' <returns></returns>
        Public Overrides Function Neighbor(base As clsPoint) As clsPoint
            Dim temp As New clsPoint(base)
            Dim coeff As Double = Math.Abs(2.0 * NeighborRange)
            For i As Integer = 0 To temp.Count - 1
                Dim tempNeighbor = coeff * Random.NextDouble() - NeighborRange
                temp(i) += tempNeighbor
            Next
            temp.ReEvaluate()

            Return temp
        End Function
    End Class

    ''' <summary>
    ''' Simulated Annealing
    ''' </summary>
    ''' <remarks>
    ''' Features:
    '''  -Randomized algorithm for optimization.
    ''' 
    ''' Reffrence:
    ''' http://ja.wikipedia.org/wiki/%E7%84%BC%E3%81%8D%E3%81%AA%E3%81%BE%E3%81%97%E6%B3%95
    ''' 
    ''' Implment:
    ''' N.Tomi(tomi.nori+github at gmail.com)
    ''' </remarks>
    Public Class clsOptSimulatedAnnealing : Inherits absOptimization
#Region "Member"
        ''' <summary>Max iteration count(Default:20,000)</summary>
        Public Overrides Property Iteration As Integer = 20000

        '-------------------------------------------------------------------
        'Coefficient of SA(Simulated Annealing)
        '-------------------------------------------------------------------
        ''' <summary>start temperature(default:1.0)</summary>
        Public Property Temperature As Double = 1

        ''' <summary>cooling ratio(default:0.9995)</summary>
        Public Property CoolingRatio As Double = 0.9995

        ''' <summary>stop temperature(default:0.0001)</summary>
        Public Property StopTemperature As Double = 0.0001

        ''' <summary>Neighbor</summary>
        Public Property Neighbor As absNeighbor = New LocalRandomSearch()

        ''' <summary>now temperature</summary>
        Private _nowTemprature As Double = 0.0

        ''' <summary>best point</summary>
        Private m_point As clsPoint = Nothing

        ''' <summary>best point</summary>
        Private m_Bestpoint As clsPoint = Nothing

#End Region

#Region "Constructor"
        ''' <summary>
        ''' Default constructor
        ''' </summary>
        ''' <remarks></remarks>
        Public Sub New(ByVal ai_func As absObjectiveFunction)
            Me.m_func = ai_func
            Me.m_point = New clsPoint(ai_func)
        End Sub
#End Region

#Region "Public"
        ''' <summary>
        ''' Initialize
        ''' </summary>
        ''' <remarks></remarks>
        Public Overrides Sub Init()
            Try
                Me.m_iteration = 0
                Me.m_point.Clear()

                'init Temperature
                Me._nowTemprature = Me.Temperature

                'init initial position
                If InitialPosition IsNot Nothing AndAlso InitialPosition.Length = m_func.NumberOfVariable Then
                    Me.m_point = New clsPoint(Me.m_func, InitialPosition)
                Else
                    Dim array = clsUtil.GenRandomPositionArray(Me.m_func, InitialPosition, Me.InitialValueRangeLower, Me.InitialValueRangeUpper)
                    Me.m_point = New clsPoint(Me.m_func, array)
                End If

                Me.m_Bestpoint = Me.m_point.Copy()
            Catch ex As Exception
                Me.m_error.SetError(True, Util.clsError.ErrorType.ERR_INIT)
            Finally
                System.GC.Collect()
            End Try
        End Sub

        ''' <summary>
        ''' Initialize(for restart)
        ''' </summary>
        ''' <param name="point">reserve best point</param>
        Public Overloads Sub Init(ByVal point As clsPoint)
            Try
                Dim tempPoint = point.Copy()
                Me.Init()
                Me.m_Bestpoint = tempPoint
            Catch ex As Exception
                Me.m_error.SetError(True, Util.clsError.ErrorType.ERR_INIT)
            Finally
                System.GC.Collect()
            End Try
        End Sub

        ''' <summary>
        ''' Do optimization
        ''' </summary>
        ''' <param name="ai_iteration"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overrides Function DoIteration(Optional ai_iteration As Integer = 0) As Boolean
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

                'neighbor function(genereate neighbor point from now point)
                Dim tempNext As clsPoint = Me.Neighbor.Neighbor(Me.m_point)
                tempNext.ReEvaluate()

                'transition
                Dim evalNow As Double = Me.m_point.Eval
                Dim evalNext As Double = tempNext.Eval

                'cooling
                If Me._nowTemprature > StopTemperature Then
                    Me._nowTemprature *= Me.CoolingRatio
                Else
                    If Me.IsUseCriterion = True Then
                        Return True
                    End If
                End If

                Dim r1 As Double = 0.0
                If evalNext < evalNow Then
                    r1 = 1.0
                Else
                    Dim delta = evalNow - evalNext
                    r1 = Math.Exp(delta / Me._nowTemprature)
                End If

                'compare probabilities
                Dim r2 = MyBase.Random.NextDouble()
                If r1 >= r2 Then
                    Me.m_point = tempNext
                End If

                'reserve best
                If Me.m_point.Eval < Me.m_Bestpoint.Eval Then
                    Me.m_Bestpoint = Me.m_point.Copy()
                End If
            Next

            Return False
        End Function

        ''' <summary>
        ''' Recent Error
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overrides Function IsRecentError() As Boolean
            Return Me.m_error.IsError()
        End Function

        ''' <summary>
        ''' Result
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overrides ReadOnly Property Result As Optimization.clsPoint
            Get
                Return Me.m_Bestpoint
            End Get
        End Property

        ''' <summary>
        ''' for Debug
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overrides ReadOnly Property Results As List(Of Optimization.clsPoint)
            Get
                Return New List(Of clsPoint)({Me.m_Bestpoint})
            End Get
        End Property
#End Region
    End Class
End Namespace
