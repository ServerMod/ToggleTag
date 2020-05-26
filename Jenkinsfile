pipeline {
  agent any
  stages {
    stage('Dependencies') {
      steps {
        sh 'nuget restore ToggleTag.sln'
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
        sh 'mv ToggleTag/bin/YamlDotNet.dll Plugin/dependencies'
        sh 'mv ToggleTag/bin/Newtonsoft.Json.dll Plugin/dependencies'
      }
    }
    stage('Archive') {
      steps {
        sh 'zip -r ToggleTag.zip Plugin'
        archiveArtifacts(artifacts: 'ToggleTag.zip', onlyIfSuccessful: true)
      }
    }
  }
}