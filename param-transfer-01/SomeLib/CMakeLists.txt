include(GenerateExportHeader)

set(LIBHELLO_SRC hello.c)

add_library(hello SHARED ${LIBHELLO_SRC})

generate_export_header(hello)

target_include_directories(hello
    PUBLIC
        "$<BUILD_INTERFACE:${CMAKE_CURRENT_BINARY_DIR}>"
)
