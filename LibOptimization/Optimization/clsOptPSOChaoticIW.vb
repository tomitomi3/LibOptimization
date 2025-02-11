Imports LibOptimization.Util
Imports LibOptimization.MathTool

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
    <Serializable>
    Public Class clsOptPSOChaoticIW : Inherits absOptimization
#Region "Member"
        ''' <summary>Max Iteration(Default:20,000)</summary>
        Public Overrides Property Iteration As Integer = 20000

        ''' <summary>Epsilon(Default:0.000001) for Criterion</summary>
        Public Property EPS As Double = 0.000001 '1e-6

        ''' <summary>
        ''' higher N percentage particles are finished at the time of same evaluate value.
        ''' This parameter is valid is when IsUseCriterion is true.
        ''' </summary>
        Public Property HigherNPercent As Double = 0.8 'for IsCriterion()
        Private HigherNPercentIndex As Integer = 0 'for IsCriterion())

        'particles
        Private m_swarm As New List(Of clsParticle)
        Private m_globalBest As clsPoint = Nothing

        '-------------------------------------------------------------------
        'Coefficient of PSO
        '-------------------------------------------------------------------
        ''' <summary>Swarm Size(Default:100)</summary>
        Public Property SwarmSize As Integer = 100

        ''' <summary>adaptive inertia weight(Default:1.0)</summary>
        Public Property Weight As Double = 1.0

        ''' <summary>Weight max for adaptive weight(Default:0.9).</summary>
        Public Property WeightMax As Double = 0.9 'base LDIW

        ''' <summary>Weight min for adaptive weight(Default:0.4).</summary>
        Public Property WeightMin As Double = 0.4

        ''' <summary>velocity coefficient(affected by personal best)(Default:1.49445)</summary>
        Public Property C1 As Double = 1.49445

        ''' <summary>velocity coefficient(affected by global best)(Default:1.49445)</summary>
        Public Property C2 As Double = 1.49445

        ''' <summary>Inertial weight strategie</summary>
        Public Property ChaoticMode As EnumChaoticInertiaWeightMode = EnumChaoticInertiaWeightMode.CDIW

        Public Enum EnumChaoticInertiaWeightMode
            ''' <summary>Charotic Decrease Inertia Weight</summary>
            CDIW
            ''' <summary>Charotic Random Inertia Weight</summary>
            CRIW
        End Enum
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

#Region "Public"
        ''' <summary>
        ''' Initialize
        ''' </summary>
        ''' <remarks></remarks>
        Public Overrides Sub Init()
            Try
                ' メンバ変数の初期化
                Me.m_iteration = 0
                Me.m_swarm.Clear()

                ' InitialPosition のチェック
                Dim validInitial As Boolean = False
                If MyBase.InitialPosition IsNot Nothing Then
                    If MyBase.InitialPosition.Length = Me.m_func.NumberOfVariable Then
                        validInitial = True
                    Else
                        Throw New ArgumentException("The number of variavles in InitialPosition and objective function are different.")
                    End If
                End If

                Dim particleCount As Integer = Me.SwarmSize

                If validInitial Then
                    ' 有効な InitialPosition がある場合
                    For i As Integer = 0 To particleCount - 2
                        Dim array() As Double = clsUtil.GenRandomPositionArray(Me.m_func, MyBase.InitialPosition, Me.InitialValueRangeLower, Me.InitialValueRangeUpper)
                        Dim tempPosition As New clsPoint(Me.m_func, array)
                        Dim tempBestPosition As clsPoint = tempPosition.Copy()
                        Dim tempVelocity() As Double = clsUtil.GenRandomPositionArray(Me.m_func, Nothing, Me.InitialValueRangeLower, Me.InitialValueRangeUpper)
                        Me.m_swarm.Add(New clsParticle(tempPosition, tempVelocity, tempBestPosition))
                    Next

                    ' 指定された InitialPosition をそのまま利用する粒子を追加
                    Dim initPosition As New clsPoint(Me.m_func, MyBase.InitialPosition)
                    Dim initBestPosition As clsPoint = initPosition.Copy()
                    Dim initVelocity() As Double = clsUtil.GenRandomPositionArray(Me.m_func, Nothing, Me.InitialValueRangeLower, Me.InitialValueRangeUpper)
                    Me.m_swarm.Add(New clsParticle(initPosition, initVelocity, initBestPosition))
                Else
                    ' 有効な InitialPosition が指定されていない場合は、すべて乱数生成
                    For i As Integer = 0 To particleCount - 1
                        Dim array() As Double = clsUtil.GenRandomPositionArray(Me.m_func, Nothing, Me.InitialValueRangeLower, Me.InitialValueRangeUpper)
                        Dim tempPosition As New clsPoint(Me.m_func, array)
                        Dim tempBestPosition As clsPoint = tempPosition.Copy()
                        Dim tempVelocity() As Double = clsUtil.GenRandomPositionArray(Me.m_func, Nothing, Me.InitialValueRangeLower, Me.InitialValueRangeUpper)
                        Me.m_swarm.Add(New clsParticle(tempPosition, tempVelocity, tempBestPosition))
                    Next
                End If

                ' 評価値に基づいて swarm をソート
                Me.m_swarm.Sort()
                Me.m_globalBest = Me.m_swarm(0).BestPoint.Copy()
                Me.Weight = 1

                ' HigherNPercentIndex の決定
                Me.HigherNPercentIndex = CInt(Me.m_swarm.Count * Me.HigherNPercent)
                If Me.HigherNPercentIndex >= Me.m_swarm.Count Then
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
            If Me.Iteration <= m_iteration Then
                Return True
            Else
                ai_iteration = If(ai_iteration = 0, Iteration - m_iteration - 1, Math.Min(ai_iteration, Iteration - m_iteration) - 1)
            End If
            For iterate As Integer = 0 To ai_iteration
                'Counting generation
                m_iteration += 1

                'check criterion - higher N percentage particles are finished at the time of same evaluate value.
                If Me.IsUseCriterion = True AndAlso clsUtil.IsCriterion(Me.EPS, Me.m_swarm, Me.HigherNPercentIndex) Then
                    Return True
                End If

                'PSO process
                For Each particle In Me.m_swarm
                    'replace personal best, find global best
                    If particle.Point.Eval < particle.BestPoint.Eval Then
                        particle.BestPoint = particle.Point.Copy()
                    End If

                    'find globalbest
                    If particle.BestPoint.Eval < Me.m_globalBest.Eval Then
                        Me.m_globalBest = particle.BestPoint
                    End If
                Next
                Me.m_globalBest = Me.m_globalBest.Copy()

                'update a velocity 
                For Each particle In Me.m_swarm
                    For i As Integer = 0 To Me.m_func.NumberOfVariable - 1
                        Dim r1 = Me.m_rand.NextDouble()
                        Dim r2 = Me.m_rand.NextDouble()
                        Dim newV = Me.Weight * particle.Velocity(i) +
                                   C1 * r1 * (particle.BestPoint(i) - particle.Point(i)) +
                                   C2 * r2 * (Me.m_globalBest(i) - particle.Point(i))
                        particle.Velocity(i) = newV

                        'update a position using velocity
                        Dim newPos = particle.Point(i) + particle.Velocity(i)
                        particle.Point(i) = newPos
                    Next
                    particle.Point.ReEvaluate()
                Next

                'Inertia Weight Strategie
                If Me.ChaoticMode = EnumChaoticInertiaWeightMode.CDIW Then
                    'CDIW is Chaotic Descending(Decreasing?) Inertia Weight
                    Dim randVal = Me.m_rand.NextDouble()
                    Dim u = 4.0 '3.75 to 4.0
                    Dim z = u * randVal * (1 - randVal)
                    Me.Weight = (Me.WeightMax - Me.WeightMin) * (Me.Iteration - Me.m_iteration) / Me.Iteration + Me.WeightMin * z
                ElseIf Me.ChaoticMode = EnumChaoticInertiaWeightMode.CRIW Then
                    'CRIW is Chaotic Random Inertia Weight
                    Dim randVal = Me.m_rand.NextDouble()
                    Dim u = 4.0
                    Dim z = u * randVal * (1 - randVal)
                    Me.Weight = 0.5 * Me.m_rand.NextDouble() + 0.5 * z
                End If

                'sort by eval
                'Me.m_swarm.Sort()
            Next

            Return False
        End Function

        ''' <summary>
        ''' Result (return global best)
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overrides ReadOnly Property Result As Optimization.clsPoint
            Get
                'find best index
                'Dim bestIndex As Integer = 0
                'Dim bestEval = Me.m_swarm(0).BestPoint.Eval
                'For i = 0 To Me.m_swarm.Count - 1
                '    If Me.m_swarm(i).BestPoint.Eval < bestEval Then
                '        bestEval = Me.m_swarm(i).BestPoint.Eval
                '        bestIndex = i
                '    End If
                'Next
                'Return Me.m_swarm(0).BestPoint.Copy()
                Return m_swarm(clsUtil.FindCurrentBestIndexFromParticles(m_swarm)).BestPoint.Copy()
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
                Me.m_swarm.Sort()
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
