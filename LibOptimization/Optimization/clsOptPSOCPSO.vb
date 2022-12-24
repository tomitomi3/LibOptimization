﻿Imports LibOptimization.Util
Imports LibOptimization.MathTool

Namespace Optimization
    ''' <summary>
    ''' Parallel Competitive Particle Swarm Optimization
    ''' </summary>
    ''' <remarks>
    ''' Features:
    '''  -Swarm Intelligence algorithm.
    ''' 
    ''' Refference:
    ''' [1]Luu, Keurfon, et al. "A parallel competitive Particle Swarm Optimization for non-linear first arrival traveltime tomography and uncertainty quantification." Computers and Geosciences 113 (2018): 81-93.
    ''' </remarks>
    <Serializable>
    Public Class clsOptPSOCPSO : Inherits absOptimization
#Region "Member"
        ''' <summary>Max iteration count(Default:20,000)</summary>
        Public Overrides Property Iteration As Integer = 20000

        ''' <summary>Epsilon(Default:0.000001) for Criterion</summary>
        Public Property EPS As Double = 0.000001 '1e-6

        ''' <summary>
        ''' higher N percentage particles are finished at the time of same evaluate value.
        ''' This parameter is valid is when IsUseCriterion is true.
        ''' </summary>
        Public Property HigherNPercent As Double = 0.8 'for IsCriterion()
        Private HigherNPercentIndex As Integer = 0 'for IsCriterion())

        ''' <summary>particles</summary>
        Private m_swarm As New List(Of clsParticle)

        ''' <summary>global best</summary>
        Private m_globalBest As clsPoint = Nothing

        '-------------------------------------------------------------------
        'Coefficient of PSO
        '-------------------------------------------------------------------
        ''' <summary>Swarm Size(Default:100)</summary>
        Public Property SwarmSize As Integer = 100

        ''' <summary>Inertia weight. default 0.7298</summary>
        Public Property Weight As Double = 0.7298

        ''' <summary>velocity coefficient(affected by personal best). default 1.49618</summary>
        Public Property C1 As Double = 1.49618

        ''' <summary>velocity coefficient(affected by global best). default 1.49618</summary>
        Public Property C2 As Double = 1.49618

        ''' <summary>Gamma</summary>
        Public Property Gamma As Double = 1.0

        ''' <summary>Delta</summary>
        Private Delta As Double = 0.0

        ''' <summary>Zero Velocity Initialization(Engelbrecht 2012). default:true.</summary>
        Public Property IsUseZeroVelocityInitialization As Boolean = True
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
                'init meber varibles
                Me.m_iteration = 0
                Me.m_swarm.Clear()

                'init position
                For i As Integer = 0 To Me.SwarmSize - 1
                    'position
                    Dim tempPosition = New clsPoint(Me.m_func)
                    Dim tempBestPosition = New clsPoint(Me.m_func)
                    Dim array = clsUtil.GenRandomPositionArray(Me.m_func, InitialPosition, Me.InitialValueRangeLower, Me.InitialValueRangeUpper)
                    tempPosition = New clsPoint(Me.m_func, array)
                    tempBestPosition = tempPosition.Copy()

                    'velocity
                    Dim tempVelocity = New Double(Me.m_func.NumberOfVariable - 1) {}
                    If IsUseZeroVelocityInitialization = False Then
                        tempVelocity = clsUtil.GenRandomPositionArray(Me.m_func, Nothing, Me.InitialValueRangeLower, Me.InitialValueRangeUpper)
                    End If

                    'create
                    Me.m_swarm.Add(New clsParticle(tempPosition, tempVelocity, tempBestPosition))
                Next

                'Sort Evaluate
                Me.m_swarm.Sort()
                Me.m_globalBest = Me.m_swarm(0).BestPoint.Copy()

                'Swarm maximum radius
                Me.Delta = Math.Log(1.0 + 0.003 * m_swarm.Count) / Math.Max(0.2, Math.Log(0.01 * Iteration))

                'Detect HigherNPercentIndex
                Me.HigherNPercentIndex = CInt(Me.m_swarm.Count * Me.HigherNPercent)
                If Me.HigherNPercentIndex = Me.m_swarm.Count OrElse Me.HigherNPercentIndex >= Me.m_swarm.Count Then
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
                'If Me.IsUseCriterion = True AndAlso clsUtil.IsCriterion(Me.EPS, Me.m_globalBest, Me.m_swarm(Me.HigherNPercentIndex).BestPoint) Then
                '    Return True
                'End If

                'PSO process
                'update a velocity 
                Dim max_dist = 0.0
                Dim tempGlobalBest As clsPoint = m_swarm(0).BestPoint.Copy()
                For Each particle In Me.m_swarm
                    Dim temp_dist = 0.0
                    For i As Integer = 0 To Me.m_func.NumberOfVariable - 1
                        Dim r1 = Me.m_rand.NextDouble()
                        Dim r2 = Me.m_rand.NextDouble()
                        Dim newV = Me.Weight * particle.Velocity(i) +
                                   C1 * r1 * (particle.BestPoint(i) - particle.Point(i)) +
                                   C2 * r2 * (Me.m_globalBest(i) - particle.Point(i))
                        particle.Velocity(i) = newV

                        'update a position using velocity
                        particle.Point(i) = particle.Point(i) + particle.Velocity(i)

                        'distance
                        temp_dist += Math.Pow((particle.Point(i) - Me.m_globalBest(i)), 2)
                        'distance
                        'temp_dist += Math.Pow((particle.Point(i) - particle.BestPoint(i)), 2)
                    Next
                    particle.Point.ReEvaluate()

                    temp_dist = Math.Sqrt(temp_dist)
                    If temp_dist > max_dist Then
                        max_dist = temp_dist
                    End If

                    'replace personal best
                    If particle.Point.Eval < particle.BestPoint.Eval Then
                        particle.BestPoint = particle.Point.Copy()
                    End If

                    'find global
                    If particle.BestPoint.Eval < tempGlobalBest.Eval Then
                        tempGlobalBest = particle.BestPoint
                    End If
                Next

                'update globalbest
                Me.m_globalBest = tempGlobalBest.Copy()

                'restart patricles
                Dim swarm_radius = max_dist / Math.Sqrt(4.0 * m_func.NumberOfVariable())
                If swarm_radius < Me.Delta Then
                    Dim inorm = Me.m_iteration / Me.Iteration
                    inorm = 0.9
                    Dim nw = CInt(((m_swarm.Count - 1.0) / (1 + Math.Exp(1.0 / 0.09 * (inorm - Gamma + 0.5)))))
                    If nw > 0 Then
                        Me.m_swarm.Sort()
                        Dim resetSwarmSize = m_swarm.Count - nw - 1
                        For i As Integer = resetSwarmSize To m_swarm.Count - 1
                            'Particles initial positions
                            Dim tempPosition = New clsPoint(Me.m_func)
                            Dim tempBestPosition = New clsPoint(Me.m_func)
                            Dim tempRandArray = clsUtil.GenRandomPositionArray(Me.m_func, InitialPosition, Me.InitialValueRangeLower, Me.InitialValueRangeUpper)
                            tempPosition = New clsPoint(Me.m_func, tempRandArray)

                            'Pbest
                            tempBestPosition = tempPosition.Copy()
                            tempBestPosition.SetEval(Double.MaxValue)

                            'Initialize particle velocity
                            Dim tempVelocity = New Double(Me.m_func.NumberOfVariable - 1) {}
                            If IsUseZeroVelocityInitialization = False Then
                                tempVelocity = clsUtil.GenRandomPositionArray(Me.m_func, Nothing, Me.InitialValueRangeLower, Me.InitialValueRangeUpper)
                            End If
                            m_swarm(i) = New clsParticle(tempPosition, tempVelocity, tempBestPosition)
                        Next
                    End If
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
                Dim ret As New List(Of clsPoint)(Me.m_swarm.Count - 1)
                For Each p In Me.m_swarm
                    ret.Add(p.BestPoint.Copy())
                Next
                ret.Sort()
                Return ret
            End Get
        End Property
#End Region

#Region "Private"
#End Region
    End Class
End Namespace
