import * as React from 'react';
import { useState } from 'react';
import { getApi } from './api';

function App() {
    const api = getApi();

    const [file, setFile] = useState<File>()
    const searcRef = React.useRef<HTMLInputElement>();

    const handleChange = (event: any) => {
        setFile(event.target.files[0]);
    };

    const handleSubmit = (event: any) => {
        event.preventDefault();

        if (!file) return;

        const formData = new FormData();

        formData.append("file", file);
        api.auction.uploadFile(formData)
            .then(response => {
                console.log(response);
                alert("Файл успешно загружен и обработан! Невероятный успех!");
            })
            .catch(response => {
                console.log(response);
                alert("Во время загрузки файла произошла ошибка!")
            });
    };

    const loadData = () => {
        api.auction.loadData(searcRef?.current?.value === "" ? "123" : searcRef?.current?.value)
            .then(data => {
                console.log(data);
                alert("Запрос выполнен! Результат смотреть в консоли");
            })
            .catch(response => {
                console.log(response);
                alert("Не удалось выполнить запрос")
            });
    }

    return (
        <>
            <form onSubmit={handleSubmit}>
                <div>{'Загрузка JSON файла'}</div>
                <br />
                <input type="file" accept="application/json" onChange={handleChange} />
                <button type="submit">Загрузить</button>
            </form>
            <br />
            <div>Тест запроса</div>
            <br />
            <input type="text" ref={searcRef}></input>
            <button onClick={loadData}>Выполнить запрос</button>
        </>
    );
}
export default App;