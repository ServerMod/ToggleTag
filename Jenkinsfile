pipeline {
  agent any
  stages {
    stage('Dependencies') {
      steps {
        sh 'nuget restore ToggleTag.sln'
      }
    }
    stage('Use upstream Smod') {
        when { triggeredBy 'BuildUpstreamCause' }
        steps {
            sh ('rm ToggleTag/lib/Assembly-CSharp.dll')
            sh ('rm ToggleTag/lib/Smod2.dll')
            sh ('ln -s $SCPSL_LIBS/Assembly-CSharp.dll ToggleTag/lib/Assembly-CSharp.dll')
            sh ('ln -s $SCPSL_LIBS/Smod2.dll ToggleTag/lib/Smod2.dll')
        }
    }
    stage('Build') {
      steps {
        sh 'msbuild ToggleTag/ToggleTag.csproj -restore -p:PostBuildEvent='
      }
    }
    stage('Setup Output Dir') {
      steps {
        sh 'mkdir Plugin'
        sh 'mkdir Plugin/dependencies'
      }
    }
    stage('Package') {
      steps {
        sh 'mv ToggleTag/bin/ToggleTag.dll Plugin/'
        sh 'mv ToggleTag/bin/Newtonsoft.Json.dll Plugin/dependencies'
      }
    }
    stage('Archive') {
        when { not { triggeredBy 'BuildUpstreamCause' } }
        steps {
            sh 'zip -r ToggleTag.zip Plugin/*'
            archiveArtifacts(artifacts: 'ToggleTag.zip', onlyIfSuccessful: true)
        }
    }
    stage('Send upstream') {
        when { triggeredBy 'BuildUpstreamCause' }
        steps {
            sh 'zip -r ToggleTag.zip Plugin/*'
            sh 'cp ToggleTag.zip $PLUGIN_BUILDER_ARTIFACT_DIR'
        }
    }
  }
}