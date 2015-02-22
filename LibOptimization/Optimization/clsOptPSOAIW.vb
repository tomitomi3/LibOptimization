Imports LibOptimization.Util
Imports LibOptimization.MathUtil

Namespace Optimization
    ''' <summary>
    ''' Particle Swarm Optmization algorithm with adaptive inertia weight(AIWPSO)
    ''' AdaptW
    ''' </summary>
    ''' <remarks>
    ''' Features:
    '''  -Swarm Intelligence algorithm.
    '''  -Derivative free optimization algorithm.
    ''' 
    ''' Refference:
    ''' [1]A. Nickabadi, M. M. Ebadzadeh, and R. Safabakhsh, “A novel particle swarm optimization algorithm with adaptive inertia weight,” Applied Soft Computing Journal, vol. 11, no. 4, pp. 3658–3670, 2011.
    ''' </remarks>
    Public Class clsOptPSOAIW : Inherits absOptimization
#Region "Member"
        'Common parameters
        Private EPS As Double = 0.000001 '1e-6
        Private MAX_ITERATION As Integer = 20000
        Private INIT_PARAM_RANGE As Double = 5.12 'This Parameter to use when generate a variable
        Private IsUseCriterion As Boolean = True

        'particles
        Private m_swarm As New List(Of clsParticle)

        'PSO Parameters
        Private SwarmSize As Integer = 100
        Private Weight As Double = 1 'adaptive inertia weight
        Private WeightMax As Double = 1.0
        Private WeightMin As Double = 0.0
        Private C1 As Double = 1.49445
        Private C2 As Double = 1.49445

        'ErrorManage
        Private m_error As New clsError
#End Region

#Region "Constructor"
        ''' <summary>
        ''' Default constructor
        ''' </summary>
        ''' <remarks></remarks>
        Public Sub New(ByVal ai_func As absObjectiveFunction)
            Me.m_func = ai_func
        End Sub
#End Region

#Region "Property(Parameter setting)"
        ''' <summary>
        ''' epsilon
        ''' </summary>
        ''' <value></value>
        ''' <remarks>Common parameter</remarks>
        Public WriteOnly Property PARAM_EPS As Double
            Set(value As Double)
                Me.EPS = value
            End Set
        End Property

        ''' <summary>
        ''' Max iteration count
        ''' </summary>
        ''' <value></value>
        ''' <remarks>Common parameter</remarks>
        Public WriteOnly Property PARAM_MAX_ITERATION As Integer
            Set(value As Integer)
                Me.MAX_ITERATION = value
            End Set
        End Property

        ''' <summary>
        ''' Range of initial value
        ''' </summary>
        ''' <value></value>
        ''' <remarks>Common parameter</remarks>
        Public WriteOnly Property PARAM_InitRange As Double
            Set(value As Double)
                Me.INIT_PARAM_RANGE = value
            End Set
        End Property

        ''' <summary>
        ''' Use criterion
        ''' </summary>
        ''' <value></value>
        ''' <remarks>Common parameter</remarks>
        Public WriteOnly Property PARAM_IsUseCriterion As Boolean
            Set(value As Boolean)
                Me.IsUseCriterion = value
            End Set
        End Property

        ''' <summary>
        ''' Swarm Size
        ''' </summary>
        ''' <value></value>
        ''' <remarks></remarks>
        Public WriteOnly Property PARAM_Size As Integer
            Set(value As Integer)
                Me.SwarmSize = value
            End Set
        End Property

        ''' <summary>
        ''' Weight max for adaptive weight.
        ''' default 1.0
        ''' </summary>
        ''' <value></value>
        ''' <remarks>
        ''' </remarks>
        Public WriteOnly Property PARAM_WeightMax As Double
            Set(value As Double)
                Me.WeightMax = value
            End Set
        End Property

        ''' <summary>
        ''' Weight min for adaptive weight.
        ''' default 0.0
        ''' </summary>
        ''' <value></value>
        ''' <remarks>
        ''' </remarks>
        Public WriteOnly Property PARAM_WeightMin As Double
            Set(value As Double)
                Me.WeightMin = value
            End Set
        End Property

        ''' <summary>
        ''' velocity coefficient(affected by personal best).
        ''' default 1.49445
        ''' </summary>
        ''' <value></value>
        ''' <remarks></remarks>
        Public WriteOnly Property PARAM_C1 As Double
            Set(value As Double)
                Me.C1 = value
            End Set
        End Property

        ''' <summary>
        ''' velocity coefficient(affected by global best)
        ''' default 1.49445
        ''' </summary>
        ''' <value></value>
        ''' <remarks></remarks>
        Public WriteOnly Property PARAM_C2 As Double
            Set(value As Double)
                Me.C2 = value
            End Set
        End Property
#End Region

#Region "Public"
        ''' <summary>
        ''' Initialize
        ''' </summary>
        ''' <remarks></remarks>
        Public Overrides Sub Init()
            Try
                'init meber varibles
                Me.m_iteration = 0
                Me.m_swarm.Clear()

                'Set initialize value
                For i As Integer = 0 To Me.SwarmSize - 1
                    Dim tempPosition = New clsPoint(Me.m_func)
                    Dim tempBestPosition = New clsPoint(Me.m_func)
                    Dim tempVelocity(Me.m_func.NumberOfVariable - 1) As Double
                    For j As Integer = 0 To Me.m_func.NumberOfVariable - 1
                        tempPosition(j) = Math.Abs(2.0 * INIT_PARAM_RANGE) * m_rand.NextDouble() - INIT_PARAM_RANGE
                        tempBestPosition(j) = tempPosition(j)
                        tempVelocity(j) = Math.Abs(2.0 * INIT_PARAM_RANGE) * m_rand.NextDouble() - INIT_PARAM_RANGE
                    Next
                    tempPosition.ReEvaluate()
                    tempBestPosition.ReEvaluate()
                    Me.m_swarm.Add(New clsParticle(tempPosition, tempVelocity, tempBestPosition))
                Next

                'Sort Evaluate
                Me.m_swarm.Sort()
                Me.Weight = 1

            Catch ex As Exception
                Me.m_error.SetError(True, Util.clsError.ErrorType.ERR_INIT)
            Finally
                System.GC.Collect()
            End Try
        End Sub

        ''' <summary>
        ''' Do optimize
        ''' </summary>
        ''' <param name="ai_iteration"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overrides Function DoIteration(Optional ai_iteration As Integer = 0) As Boolean
            'Check Last Error
            If Me.IsRecentError() = True Then
                Return True
            End If

            'do iterate
            ai_iteration = If(ai_iteration = 0, Me.MAX_ITERATION - 1, ai_iteration - 1)
            For iterate As Integer = 0 To ai_iteration
                'check iteration count
                If MAX_ITERATION <= m_iteration Then
                    Me.m_swarm.Sort()
                    Me.m_error.SetError(True, Util.clsError.ErrorType.ERR_OPT_MAXITERATION)
                    Return True
                End If
                m_iteration += 1

                'get global best
                Me.m_swarm.Sort()
                Dim globalBestPoint As clsPoint = Me.m_swarm(0).BestPoint.Copy()

                'check criterion
                If Me.IsUseCriterion = True Then
                    If clsUtil.IsCriterion(Me.EPS, Me.m_swarm(0).BestPoint, Me.m_swarm(Me.SwarmSize - 1).BestPoint) Then
                        Return True
                    End If
                End If

                Dim replaceBestCount As Integer = 0
                For Each particle In Me.m_swarm
                    'update a velocity 
                    For i As Integer = 0 To Me.m_func.NumberOfVariable - 1
                        Dim r1 = Me.m_rand.NextDouble()
                        Dim r2 = Me.m_rand.NextDouble()
                        Dim newV = Me.Weight * particle.Velocity(i) + _
                                   C1 * r1 * (particle.BestPoint(i) - particle.Point(i)) + _
                                   C2 * r2 * (globalBestPoint(i) - particle.Point(i))
                        particle.Velocity(i) = newV

                        'update a position using velocity
                        Dim newPos = particle.Point(i) + particle.Velocity(i)
                        particle.Point(i) = newPos
                    Next
                    particle.Point.ReEvaluate()

                    'replace personal best
                    If particle.Point.Eval < particle.BestPoint.Eval Then
                        particle.BestPoint = particle.Point.Copy()
                        replaceBestCount += 1 'for AIWPSO

                        'replace global best
                        If particle.Point.Eval < globalBestPoint.Eval Then
                            globalBestPoint = particle.Point.Copy()
                        End If
                    End If
                Next

                'AIWPSO
                Dim PS = replaceBestCount / Me.SwarmSize
                Me.Weight = (Me.WeightMax - Me.WeightMin) * PS - Me.WeightMin
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
                Return Me.m_swarm(0).Point
            End Get
        End Property

        ''' <summary>
        ''' for Debug
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overrides ReadOnly Property ResultForDebug As List(Of Optimization.clsPoint)
            Get
                Dim ret As New List(Of clsPoint)(Me.m_swarm.Count - 1)
                For Each p In Me.m_swarm
                    ret.Add(p.BestPoint.Copy())
                Next
                Return ret
            End Get
        End Property
#End Region

#Region "Private"
#End Region
    End Class
End Namespace
