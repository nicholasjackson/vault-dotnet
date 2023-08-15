variable "runtimes" {
  default = {
    osx = {
      arm = "osx.13-arm64"
    }
  }
}

resource "build" "app" {
  container {
    dockerfile = "Dockerfile.build"
    context    = "../app"

    args = {
      runtime = variable.runtimes.osx.arm
    }
  }

  //output {
  //  # source file or directory in the container
  //  source = "/src"
  //  # destination folder on local filesystem
  //  destination = "./build"
  //}
  
  output {
    # source file or directory in the container
    source = "/src/bin/Debug/net7.0/${variable.runtimes.osx.arm}/publish"
    # destination folder on local filesystem
    destination = "../bin"
  }
}