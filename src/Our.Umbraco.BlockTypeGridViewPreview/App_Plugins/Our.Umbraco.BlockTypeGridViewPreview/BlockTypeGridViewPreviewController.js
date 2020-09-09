
angular.module("umbraco").controller("BlockTypeGridViewPreviewController", [

    "$scope",
    "editorState",
    "$http",

    function ($scope, editorState, $http) {

        let model = $scope.$parent.$parent.$parent.$parent.$parent.$parent.$parent.$parent.model;
        let currentState = angular.copy(editorState.current);
        let propertyAlias = model.alias;
        let editorAlias = model.editor;


        let url = Umbraco.Sys.ServerVariables.umbracoSettings.umbracoPath + "/backoffice/BlockTypeGridViewPreview/Preview/GetBlockPreviewMarkup";
        $scope.setPreview = function (block) {

            let value = {
                layout: {},
                contentData: model.value.contentData.filter(c => c.udi == block.data?.udi),
                settingsData: model.value.settingsData.filter(c => c.udi == block.settingsData?.udi)
            };
            value.layout[editorAlias] = [{
                'contentUdi': block.data?.udi,
                'settingsUdi': block.settingsData?.udi
            }];

            console.log({ model, block, value });

            $http({
                method: 'POST',
                url: url,
                data: $.param({
                    pageId: currentState.id,
                    propertyAlias: propertyAlias,
                    contentTypeAlias: currentState.contentTypeAlias,
                    value: JSON.stringify(value)
                }),
                headers: {
                    'Content-Type': 'application/x-www-form-urlencoded'
                }
            }).then(function (response) {
                let htmlResult = response.data;
                if (htmlResult.trim().length > 0) {
                    $scope.preview = htmlResult;
                }
            });
        };

        $scope.setPreview($scope.block);

        $scope.$watch('block.data', function (newVal, oldVal) {
            $scope.setPreview($scope.block);
        }, true);

    }
]);