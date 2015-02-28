Imports LibOptimization.Util
Imports LibOptimization.MathUtil

Namespace Optimization
    ''' <summary>
    ''' Particle Swarm Optimization using Chaotic inertia weight(CDIW-PSO, CRIW-PSO)
    ''' </summary>
    ''' <remarks>
    ''' Features:
    '''  -Swarm Intelligence algorithm.
    '''  -Derivative free optimization algorithm.
    ''' 
    ''' Refference:
    ''' [1]Y. Feng, G. Teng, A. Wang, Y.M. Yao, "Chaotic inertia weight in particle swarm optimization", in: Second International Conference on Innovative Computing, Information and Control (ICICIC 07), 2007, pp. 475–1475.
    ''' </remarks>
    Public Class clsOptPSOChaoticIW : Inherits absOptimization
#Region "Member"
        'Common parameters
        Private EPS As Double = 0.000001 '1e-6
        Private MAX_ITERATION As Integer = 20000
        Private INIT_PARAM_RANGE As Double = 5.12 'This Parameter to use when generate a variable
        Private IsUseCriterion As Boolean = True
        Private HigherNPercent As Double = 0.9 'for IsCriteorion()
        Private HigherNPercentIndex As Integer = 0 'for IsCriterion())

        'particles
        Private m_swarm As New List(Of clsParticle)
        Private m_globalBest As clsPoint = Nothing

        'PSO Parameters
        Private SwarmSize As Integer = 100
        Private Weight As Double = 1
        Private WeightMax As Double = 0.9 'base LDIW
        Private WeightMin As Double = 0.4
        Private C1 As Double = 1.49445
        Private C2 As Double = 1.49445
        Private ChaoticMode As EnumChaoticInertiaWeightMode = EnumChaoticInertiaWeightMode.CDIW

        Public Enum EnumChaoticInertiaWeightMode
            ''' <summary>Charotic Decrease Inertia Weight</summary>
            CDIW
            ''' <summary>Charotic Random Inertia Weight</summary>
            CRIW
        End Enum

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
        ''' higher N percentage particles are finished at the time of same evaluate value.
        ''' This parameter is valid is when PARAM_IsUseCriterion is true.
        ''' </summary>
        ''' <value></value>
        ''' <remarks>Common parameter</remarks>
        Public WriteOnly Property PARAM_CriterionPersent As Double
            Set(value As Double)
                Me.HigherNPercent = value
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
        ''' default 0.9
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
        ''' default 0.4
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

        ''' <summary>
        ''' Inertial weight strategie
        ''' </summary>
        ''' <value></value>
        ''' <remarks></remarks>
        Public WriteOnly Property PARAM_InertialWeightStrategie As EnumChaoticInertiaWeightMode
            Set(value As EnumChaoticInertiaWeightMode)
                Me.ChaoticMode = value
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
                Me.m_globalBest = Me.m_swarm(0).BestPoint.Copy()
                Me.Weight = 1

                'Detect HigherNPercentIndex
                Me.HigherNPercentIndex = CInt(Me.m_swarm.Count * Me.HigherNPercent)
                If Me.HigherNPercentIndex = Me.m_swarm.Count Then
                    Me.HigherNPercentIndex = Me.m_swarm.Count - 1
                End If

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

                'check criterion
                If Me.IsUseCriterion = True Then
                    'higher N percentage particles are finished at the time of same evaluate value.
                    If clsUtil.IsCriterion(Me.EPS, Me.m_swarm(0).BestPoint, Me.m_swarm(Me.HigherNPercentIndex).BestPoint) Then
                        Return True
                    End If
                End If

                'PSO process
                For Each particle In Me.m_swarm
                    'update a velocity 
                    For i As Integer = 0 To Me.m_func.NumberOfVariable - 1
                        Dim r1 = Me.m_rand.NextDouble()
                        Dim r2 = Me.m_rand.NextDouble()
                        Dim newV = Me.Weight * particle.Velocity(i) + _
                                   C1 * r1 * (particle.BestPoint(i) - particle.Point(i)) + _
                                   C2 * r2 * (Me.m_globalBest(i) - particle.Point(i))
                        particle.Velocity(i) = newV

                        'update a position using velocity
                        Dim newPos = particle.Point(i) + particle.Velocity(i)
                        particle.Point(i) = newPos
                    Next
                    particle.Point.ReEvaluate()

                    'replace personal best
                    If particle.Point.Eval < particle.BestPoint.Eval Then
                        particle.BestPoint = particle.Point.Copy()

                        'replace global best
                        If particle.Point.Eval < Me.m_globalBest.Eval Then
                            Me.m_globalBest = particle.Point.Copy()
                        End If
                    End If
                Next

                'Inertia Weight Strategie
                If Me.ChaoticMode = EnumChaoticInertiaWeightMode.CDIW Then
                    'CDIW is Chaotic Descending(Decreasing?) Inertia Weight
                    Dim randVal = Me.m_rand.NextDouble()
                    Dim u = 4.0 '3.75 to 4.0
                    Dim z = u * randVal * (1 - randVal)
                    Me.Weight = (Me.WeightMax - Me.WeightMin) * (Me.MAX_ITERATION - Me.m_iteration) / Me.MAX_ITERATION + Me.WeightMin * z
                ElseIf Me.ChaoticMode = EnumChaoticInertiaWeightMode.CRIW Then
                    'CRIW is Chaotic Random Inertia Weight
                    Dim randVal = Me.m_rand.NextDouble()
                    Dim u = 4.0
                    Dim z = u * randVal * (1 - randVal)
                    Me.Weight = 0.5 * Me.m_rand.NextDouble() + 0.5 * z
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
                Return Me.m_swarm(0).BestPoint
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
